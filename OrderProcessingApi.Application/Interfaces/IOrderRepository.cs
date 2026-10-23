using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Application.Interfaces;

public interface IOrderRepository
{
    Task AddOrderAsync(
        Pedido order,
        CancellationToken cancellationToken = default);

    Task<List<Pedido>> GetOrdersAsync(
        CancellationToken cancellationToken = default);

    Task<Pedido?> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}