namespace OrderProcessingApi.Application.Interfaces;

public interface IUnitOfWork
{
    IOrderRepository Orders { get; }

    IProductRepository Products { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);
}