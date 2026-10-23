
using Microsoft.AspNetCore.Mvc;
using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Interfaces;

namespace OrderProcessingApi.Api.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductsController : ControllerBase
{
    private readonly IProduct _productService;

    public ProductsController(IProduct productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetProductsAsync(
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductByIdAsync(
            id,
            cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new
            {
                mensaje = "Producto no encontrado.",
                errores = result.Errors
            });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDTOs product,
        CancellationToken cancellationToken)
    {
        var result = await _productService.CreateProductAsync(
            product,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                mensaje = "No fue posible crear el producto.",
                errores = result.Errors
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            mensaje = "Producto creado correctamente.",
            productoId = result.Value
        });
    }
}
