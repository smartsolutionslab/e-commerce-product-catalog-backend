using E_Commerce.Common.Infrastructure.Persistence;
using E_Commerce.Common.Infrastructure.Services;
using E_Commerce.ProductCatalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.ProductCatalog.Infrastructure.Persistence;

public class ProductDbContext : BaseDbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options, ITenantService tenantService, IPublisher publisher)
        : base(options, tenantService, publisher)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
