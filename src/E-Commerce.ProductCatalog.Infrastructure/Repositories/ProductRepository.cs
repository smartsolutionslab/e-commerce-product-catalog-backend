using E_Commerce.ProductCatalog.Application.Interfaces;
using E_Commerce.ProductCatalog.Domain.Entities;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.ProductCatalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Prices)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Prices)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Sku == sku.ToUpperInvariant(), cancellationToken);
    }

    public async Task<List<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Prices)
            .Include(p => p.Variants)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, CategoryId? categoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(p => p.Prices)
            .Include(p => p.Variants)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Sku.Contains(searchTerm));
        }

        if (categoryId != null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        return await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm = null, CategoryId? categoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Sku.Contains(searchTerm));
        }

        if (categoryId != null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
