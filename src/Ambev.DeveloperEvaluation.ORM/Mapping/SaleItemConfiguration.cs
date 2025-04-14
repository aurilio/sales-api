using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);
        builder.Property(si => si.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(si => si.SaleId)
            .IsRequired()
            .HasColumnType("uuid");

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Mapeamento da chave estrangeira para Product (External Identity)
        builder.Property(si => si.ProductId)
            .IsRequired()
            .HasColumnType("integer");

        // Mapeamento das propriedades denormalizadas do Product
        builder.Property(si => si.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(si => si.Quantity)
            .IsRequired()
            .HasColumnType("integer");

        builder.Property(si => si.UnitPrice)
            .IsRequired()
            .HasColumnType("numeric");

        builder.Property(si => si.Discount)
            .IsRequired()
            .HasColumnType("numeric")
            .HasDefaultValue(0);

        builder.Property(si => si.TotalAmount)
            .IsRequired()
            .HasColumnType("numeric");

        builder.Property(si => si.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(si => si.UpdatedAt)
            .HasColumnType("timestamp with time zone");
    }
}