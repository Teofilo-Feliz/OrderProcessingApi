using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Infrastructure.Configurations;

public class PedidoDetalleConfiguration : IEntityTypeConfiguration<PedidoDetalle>
{
    public void Configure(EntityTypeBuilder<PedidoDetalle> builder)
    {
        builder.ToTable("PedidoDetalles", "Ventas");

        builder.HasIndex(e => e.ProductoId, "IX_PedidoDetalles_ProductoId");

        builder.HasIndex(e => new { e.PedidoId, e.ProductoId }, "UQ_PedidoDetalles_Pedido_Producto")
            .IsUnique();

        builder.Property(e => e.PrecioUnitario)
            .HasColumnType("decimal(18, 2)");

        builder.HasOne(d => d.Pedido)
            .WithMany(p => p.PedidoDetalles)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PedidoDetalles_Pedidos");

        builder.HasOne(d => d.Producto)
            .WithMany(p => p.PedidoDetalles)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PedidoDetalles_Productos");
    }
}