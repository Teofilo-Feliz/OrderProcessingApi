using Microsoft.EntityFrameworkCore.Storage;
using OrderProcessingApi.Infrastructure.Configuration;
using OrderProcessingApi.Application.Interfaces;


namespace OrderProcessingApi.Infrastructure.Repository.UnitOfWork.Services
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly OrderProcessingApiDbContext _context;

        public UnitOfWork(
            OrderProcessingApiDbContext context,
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _context = context;
            Orders = orderRepository;
            Products = productRepository;
        }

        public IOrderRepository Orders { get; }

        public IProductRepository Products { get; }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            await using IDbContextTransaction transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                T result = await operation(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

    }
}
