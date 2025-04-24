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

        builder.Property(si => si.ProductId)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.Discount)
            .IsRequired()
            .HasColumnType("numeric");

        builder.Property(si => si.TotalAmount)
            .IsRequired()
            .HasColumnType("numeric");

        builder.Property(si => si.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(si => si.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.OwnsOne(si => si.ProductDetails, pd =>
        {
            pd.Property(p => p.Title)
              .HasColumnName("ProductTitle")
              .HasMaxLength(255)
              .IsRequired();

            pd.Property(p => p.Category)
              .HasColumnName("ProductCategory")
              .HasMaxLength(100)
              .IsRequired();

            pd.Property(p => p.Price)
              .HasColumnName("ProductPrice")
              .IsRequired();

            pd.Property(p => p.Image)
              .HasColumnName("ProductImage")
              .IsRequired();

            pd.WithOwner();
        });
    }
}