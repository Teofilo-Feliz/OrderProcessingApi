using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingApi.Domain.Entities;

namespace OrderProcessingApi.Infrastructure.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos", "Ventas");

        builder.Property(e => e.Nombre).HasMaxLength(150);

        builder.Property(e => e.Precio)
            .HasColumnType("decimal(18, 2)");
    }
}