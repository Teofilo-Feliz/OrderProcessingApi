using Microsoft.EntityFrameworkCore;
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

        public async Task AddOrderAsync(
            Pedido order,
            CancellationToken cancellationToken = default)
        {
            await _context.Pedidos.AddAsync(order, cancellationToken);
        }

        public async Task<List<Pedido>> GetOrdersAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Pedidos
                .AsNoTracking()
                .OrderByDescending(p => p.FechaPedido)
                .ThenByDescending(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Pedido?> GetOrderByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Pedidos
                .AsNoTracking()
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

    }
}
