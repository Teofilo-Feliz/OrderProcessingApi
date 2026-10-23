using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderProcessingApi.Domain.Entities;
using OrderProcessingApi.Infrastructure.Configuration;
using OrderProcessingApi.Application.Interfaces;


namespace OrderProcessingApi.Infrastructure.Repository.OrderRepository.Services
{
    public class OrderRepository: IOrderRepository
    {
        private readonly OrderProcessingApiDbContext _context;

        public OrderRepository(OrderProcessingApiDbContext context)
        {
            _context = context;
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

        public async Task<List<Producto>> ObtenerProductosAsync(
            IEnumerable<int> productosIds,
            CancellationToken cancellationToken = default)
        {
            var ids = productosIds.Distinct().ToList();

            return await _context.Productos
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<int> DescontarStockAsync(
            int productoId,
            int cantidad,
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .Where(p => p.Id == productoId && p.Stock >= cantidad)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        p => p.Stock,
                        p => p.Stock - cantidad),
                    cancellationToken);
        }

        public async Task GuardarPedidoAsync(
            Pedido pedido,
            CancellationToken cancellationToken = default)
        {
            await _context.Pedidos.AddAsync(pedido, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
