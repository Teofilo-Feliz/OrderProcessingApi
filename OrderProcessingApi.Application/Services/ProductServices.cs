using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Helpers;
using OrderProcessingApi.Application.Interfaces;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Application.Services;

public class ProductService : IProduct
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProductDTOs>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products
            .GetProductsAsync(cancellationToken);

        return products
            .Select(p => new ProductDTOs(
                p.Id,
                p.Nombre,
                p.Precio,
                p.Stock))
            .ToList();
    }

    public async Task<Result<ProductDTOs>> GetProductByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products
            .GetProductByIdAsync(id, cancellationToken);

        if (product == null)
        {
            return Result<ProductDTOs>.Failure(
                $"El producto con ID {id} no existe.");
        }

        return Result<ProductDTOs>.Success(
            new ProductDTOs(
                product.Id,
                product.Nombre,
                product.Precio,
                product.Stock));
    }

    public async Task<Result<int>> CreateProductAsync(
        CreateProductDTOs product,
        CancellationToken cancellationToken = default)
    {
        if (product == null)
        {
            return Result<int>.Failure(
                "Debe especificar los datos del producto.");
        }

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(product.Nombre))
        {
            errors.Add("El nombre del producto es obligatorio.");
        }
        else if (product.Nombre.Trim().Length > 150)
        {
            errors.Add("El nombre del producto no puede superar los 150 caracteres.");
        }

        if (product.Precio <= 0)
        {
            errors.Add("El precio del producto debe ser mayor que cero.");
        }

        if (product.Stock < 0)
        {
            errors.Add("El stock del producto debe ser mayor o igual a cero.");
        }

        if (errors.Count > 0)
        {
            return Result<int>.Failure(errors);
        }

        var newProduct = new Producto
        {
            Nombre = product.Nombre.Trim(),
            Precio = product.Precio,
            Stock = product.Stock
        };

        await _unitOfWork.Products.AddProductAsync(
            newProduct,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(newProduct.Id);
    }
}
