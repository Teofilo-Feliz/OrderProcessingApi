using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Application.Interfaces;

public interface IOrderRepository
{
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);

    Task<List<Producto>> ObtenerProductosAsync(
        IEnumerable<int> productosIds,
        CancellationToken cancellationToken = default);

    Task<int> DescontarStockAsync(
        int productoId,
        int cantidad,
        CancellationToken cancellationToken = default);

    Task GuardarPedidoAsync(
        Pedido pedido,
        CancellationToken cancellationToken = default);
}