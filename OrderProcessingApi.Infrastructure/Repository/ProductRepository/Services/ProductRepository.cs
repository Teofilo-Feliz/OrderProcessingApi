using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Domain.Entities;
using OrderProcessingApi.Infrastructure.Configuration;
using OrderProcessingApi.Application.Interfaces;


namespace OrderProcessingApi.Infrastructure.Repository.ProductRepository.Services
{
    public class ProductRepository: IProductRepository
    {
        private readonly OrderProcessingApiDbContext _context;

        public ProductRepository(OrderProcessingApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetProductsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Producto?> GetProductByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Producto>> GetProductsByIdsAsync(
            IEnumerable<int> productIds,
            CancellationToken cancellationToken = default)
        {
            var ids = productIds.Distinct().ToList();

            return await _context.Productos
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<int> DecreaseStockAsync(
            int productId,
            int quantity,
            CancellationToken cancellationToken = default)
        {
            return await _context.Productos
                .Where(p => p.Id == productId && p.Stock >= quantity)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        p => p.Stock,
                        p => p.Stock - quantity),
                    cancellationToken);
        }

        public async Task AddProductAsync(
            Producto product,
            CancellationToken cancellationToken = default)
        {
            await _context.Productos.AddAsync(product, cancellationToken);
        }

    }
}
