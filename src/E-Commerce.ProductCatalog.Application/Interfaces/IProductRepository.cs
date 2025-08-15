using E_Commerce.ProductCatalog.Domain.Entities;
using E_Commerce.ProductCatalog.Domain.ValueObjects;

namespace E_Commerce.ProductCatalog.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, CategoryId? categoryId = null, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(string? searchTerm = null, CategoryId? categoryId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Remove(Product product);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(CategoryId id, CancellationToken cancellationToken = default);
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Category>> GetByParentIdAsync(CategoryId? parentId, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Update(Category category);
    void Remove(Category category);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
