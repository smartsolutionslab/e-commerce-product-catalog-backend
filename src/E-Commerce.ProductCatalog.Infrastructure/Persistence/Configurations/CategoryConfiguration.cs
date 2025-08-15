using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.Entities;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.ProductCatalog.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CategoryId.Create(value))
            .ValueGeneratedNever();

        builder.Property(c => c.TenantId)
            .HasConversion(
                id => id.Value,
                value => TenantId.Create(value))
            .IsRequired();

        builder.Property(c => c.ParentCategoryId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? CategoryId.Create(value.Value) : null);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Slug)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.SortOrder)
            .HasDefaultValue(0);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(c => new { c.TenantId, c.Slug })
            .IsUnique()
            .HasDatabaseName("IX_Categories_TenantId_Slug");

        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("IX_Categories_TenantId");
    }
}
