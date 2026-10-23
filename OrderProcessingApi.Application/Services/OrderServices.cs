using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Exceptions;
using OrderProcessingApi.Application.Helpers;
using OrderProcessingApi.Application.Interfaces;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Application.Services;

public class OrderService : IOrder
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> CreateOrderAsync(
        CreateOrderDTOs order,
        CancellationToken cancellationToken = default)
    {
        if (order?.Productos == null || order.Productos.Count == 0)
        {
            return Result<int>.Failure(
                "El pedido debe contener al menos un producto.");
        }

        if (order.Productos.Any(p =>
            p.ProductoId <= 0 || p.Cantidad <= 0))
        {
            return Result<int>.Failure(
                "Los identificadores de productos y las cantidades deben ser mayores que cero.");
        }

        var requestedProducts = order.Productos
            .GroupBy(p => p.ProductoId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(p => (long)p.Cantidad)
            })
            .ToList();

        if (requestedProducts.Any(p => p.Quantity > int.MaxValue))
        {
            return Result<int>.Failure(
                "La cantidad acumulada de un producto supera el límite permitido.");
        }

        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(
                async ct =>
                {
                    var products = await _unitOfWork.Products
                        .GetProductsByIdsAsync(
                            requestedProducts.Select(p => p.ProductId),
                            ct);

                    var productsById = products.ToDictionary(p => p.Id);

                    var rejectedProducts = new List<string>();

                    foreach (var requested in requestedProducts)
                    {
                        if (!productsById.TryGetValue(
                            requested.ProductId,
                            out var product))
                        {
                            rejectedProducts.Add(
                                $"El producto con ID {requested.ProductId} no existe.");

                            continue;
                        }

                        if (product.Stock < requested.Quantity)
                        {
                            rejectedProducts.Add(
                                $"Producto: {product.Nombre}. " +
                                $"Stock disponible: {product.Stock}. " +
                                $"Cantidad solicitada: {requested.Quantity}.");
                        }
                    }

                    if (rejectedProducts.Count > 0)
                    {
                        throw new InsufficientStockException(
                            rejectedProducts);
                    }

                    decimal total = requestedProducts.Sum(r =>
                        productsById[r.ProductId].Precio * r.Quantity);

                    var newOrder = new Pedido
                    {
                        FechaPedido = DateTime.UtcNow,
                        Total = total,
                        Estado = "Pendiente",
                        PedidoDetalles = requestedProducts
                            .Select(r => new PedidoDetalle
                            {
                                ProductoId = r.ProductId,
                                Cantidad = (int)r.Quantity,
                                PrecioUnitario =
                                    productsById[r.ProductId].Precio
                            })
                            .ToList()
                    };

                    foreach (var requested in requestedProducts)
                    {
                        int affectedRows =
                            await _unitOfWork.Products.DecreaseStockAsync(
                                requested.ProductId,
                                (int)requested.Quantity,
                                ct);

                        if (affectedRows == 0)
                        {
                            var product =
                                productsById[requested.ProductId];

                            throw new InsufficientStockException(
                                new List<string>
                                {
                                    $"No fue posible reservar el stock de " +
                                    $"{product.Nombre}. " +
                                    "El stock disponible pudo cambiar. Intenta nuevamente."
                                });
                        }
                    }

                    await _unitOfWork.Orders.AddOrderAsync(
                        newOrder,
                        ct);

                    await _unitOfWork.SaveChangesAsync(ct);

                    return Result<int>.Success(newOrder.Id);
                },
                cancellationToken);
        }
        catch (InsufficientStockException ex)
        {
            return Result<int>.Failure(ex.RejectedProducts);
        }
    }

    public async Task<List<OrderDTOs>> GetOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders
            .GetOrdersAsync(cancellationToken);

        return orders
            .Select(o => new OrderDTOs(
                o.Id,
                o.FechaPedido,
                o.Total,
                o.Estado))
            .ToList();
    }

    public async Task<Result<OrderWithDetailsDTOs>> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders
            .GetOrderByIdAsync(id, cancellationToken);

        if (order == null)
        {
            return Result<OrderWithDetailsDTOs>.Failure(
                $"El pedido con ID {id} no existe.");
        }

        return Result<OrderWithDetailsDTOs>.Success(
            new OrderWithDetailsDTOs(
                order.Id,
                order.FechaPedido,
                order.Estado,
                order.Total,
                order.PedidoDetalles
                    .Select(d => new OrderDetailDTOs(
                        d.ProductoId,
                        d.Producto.Nombre,
                        d.Cantidad,
                        d.PrecioUnitario,
                        d.PrecioUnitario * d.Cantidad))
                    .ToList()));
    }
}
