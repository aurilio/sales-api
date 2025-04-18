using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("numeric");

        builder.Property(p => p.Description)
            .HasColumnType("text");

        builder.Property(p => p.Category)
            .HasMaxLength(100);

        builder.Property(p => p.Image)
            .HasMaxLength(255);

        builder.OwnsOne(p => p.Rating, ratingBuilder =>
        {
            ratingBuilder.Property(r => r.Rate)
                .IsRequired()
                .HasColumnType("numeric");

            ratingBuilder.Property(r => r.Count)
                .IsRequired()
                .HasColumnType("integer");
        });
    }
}