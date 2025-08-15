using E_Commerce.Common.Application.Abstractions;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Application.DTOs;
using E_Commerce.ProductCatalog.Domain.ValueObjects;

namespace E_Commerce.ProductCatalog.Application.Queries;

public record GetProductsQuery(
    TenantId TenantId,
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    CategoryId? CategoryId = null) : IQuery<ProductsResponse>;

public record GetProductByIdQuery(Guid ProductId) : IQuery<ProductResponse>;

public record GetCategoriesQuery(TenantId TenantId) : IQuery<List<CategoryResponse>>;
