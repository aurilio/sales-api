using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SaleDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        // Mapeamento da chave estrangeira para Customer (External Identity)
        builder.Property(s => s.CustomerId)
            .IsRequired()
            .HasColumnType("uuid");

        // Mapeamento da propriedade denormalizada CustomerName
        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.TotalAmount)
            .IsRequired()
            .HasColumnType("numeric");

        builder.Property(s => s.Branch)
            .HasMaxLength(100);

        builder.Property(s => s.IsCancelled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.HasMany(s => s.Items)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}