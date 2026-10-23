using OrderProcessingApi.Application.DTOs;
using OrderProcessingApi.Application.Exceptions;
using OrderProcessingApi.Application.Helpers;
using OrderProcessingApi.Application.Interfaces;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Application.Services;

public class OrderService : IOrder
{
    private readonly  IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<int>> CrearPedidoAsync(
        CreateOrderDTOs pedido,
        CancellationToken cancellationToken = default)
    {
        if (pedido?.Productos == null || pedido.Productos.Count == 0)
        {
            return Result<int>.Failure(
                "El pedido debe contener al menos un producto.");
        }

        if (pedido.Productos.Any(p =>
            p.ProductoId <= 0 || p.Cantidad <= 0))
        {
            return Result<int>.Failure(
                "Los identificadores de productos y las cantidades deben ser mayores que cero.");
        }

        var productosSolicitados = pedido.Productos
            .GroupBy(p => p.ProductoId)
            .Select(g => new
            {
                ProductoId = g.Key,
                Cantidad = g.Sum(p => (long)p.Cantidad)
            })
            .ToList();

        if (productosSolicitados.Any(p => p.Cantidad > int.MaxValue))
        {
            return Result<int>.Failure(
                "La cantidad acumulada de un producto supera el límite permitido.");
        }

        try
        {
            return await _orderRepository.ExecuteInTransactionAsync(
                async ct =>
                {
                    var productos = await _orderRepository
                        .ObtenerProductosAsync(
                            productosSolicitados.Select(p => p.ProductoId),
                            ct);

                    var productosPorId = productos.ToDictionary(p => p.Id);

                    var productosRechazados = new List<string>();

                    foreach (var solicitado in productosSolicitados)
                    {
                        if (!productosPorId.TryGetValue(
                            solicitado.ProductoId,
                            out var producto))
                        {
                            productosRechazados.Add(
                                $"El producto con ID {solicitado.ProductoId} no existe.");

                            continue;
                        }

                        if (producto.Stock < solicitado.Cantidad)
                        {
                            productosRechazados.Add(
                                $"Producto: {producto.Nombre}. " +
                                $"Stock disponible: {producto.Stock}. " +
                                $"Cantidad solicitada: {solicitado.Cantidad}.");
                        }
                    }

                    if (productosRechazados.Count > 0)
                    {
                        throw new StockInsuficienteException(
                            productosRechazados);
                    }

                    decimal total = productosSolicitados.Sum(s =>
                        productosPorId[s.ProductoId].Precio * s.Cantidad);

                    var nuevoPedido = new Pedido
                    {
                        FechaPedido = DateTime.UtcNow,
                        Total = total,
                        Estado = "Pendiente",
                        PedidoDetalles = productosSolicitados
                            .Select(s => new PedidoDetalle
                            {
                                ProductoId = s.ProductoId,
                                Cantidad = (int)s.Cantidad,
                                PrecioUnitario =
                                    productosPorId[s.ProductoId].Precio
                            })
                            .ToList()
                    };

                    foreach (var solicitado in productosSolicitados)
                    {
                        int filasAfectadas =
                            await _orderRepository.DescontarStockAsync(
                                solicitado.ProductoId,
                                (int)solicitado.Cantidad,
                                ct);

                        if (filasAfectadas == 0)
                        {
                            var producto =
                                productosPorId[solicitado.ProductoId];

                            throw new StockInsuficienteException(
                                new List<string>
                                {
                                    $"No fue posible reservar el stock de " +
                                    $"{producto.Nombre}. " +
                                    "El stock disponible pudo cambiar. Intenta nuevamente."
                                });
                        }
                    }

                    await _orderRepository.GuardarPedidoAsync(
                        nuevoPedido,
                        ct);

                    return Result<int>.Success(nuevoPedido.Id);
                },
                cancellationToken);
        }
        catch (StockInsuficienteException ex)
        {
            return Result<int>.Failure(ex.ProductosRechazados);
        }
    }
}
