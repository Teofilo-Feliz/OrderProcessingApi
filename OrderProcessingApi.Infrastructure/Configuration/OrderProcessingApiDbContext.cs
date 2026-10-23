using Microsoft.EntityFrameworkCore;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Infrastructure.Configuration;

public partial class OrderProcessingApiDbContext : DbContext
{
    public OrderProcessingApiDbContext(DbContextOptions<OrderProcessingApiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Pedido> Pedidos { get; set; }
    public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderProcessingApiDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}