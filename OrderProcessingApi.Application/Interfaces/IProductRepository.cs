using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Producto>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    Task<Producto?> GetProductByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<Producto>> GetProductsByIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default);

    Task<int> DecreaseStockAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task AddProductAsync(
        Producto product,
        CancellationToken cancellationToken = default);
}