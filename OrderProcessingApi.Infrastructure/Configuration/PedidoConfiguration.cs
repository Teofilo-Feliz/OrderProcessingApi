using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Infrastructure.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos", "Ventas");

        builder.Property(e => e.Estado)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("Pendiente");

        builder.Property(e => e.FechaPedido)
            .HasPrecision(0)
            .HasDefaultValueSql("(sysdatetime())");

        builder.Property(e => e.Total)
            .HasColumnType("decimal(18, 2)");
    }
}