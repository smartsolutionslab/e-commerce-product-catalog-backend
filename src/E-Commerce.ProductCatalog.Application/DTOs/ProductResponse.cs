namespace E_Commerce.ProductCatalog.Application.DTOs;

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    Guid CategoryId,
    string Status,
    int StockQuantity,
    int MinStockLevel,
    List<ProductPriceResponse> Prices,
    List<ProductVariantResponse> Variants,
    DateTime CreatedAt);

public record ProductPriceResponse(
    Guid Id,
    decimal Amount,
    string Currency,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo);

public record ProductVariantResponse(
    Guid Id,
    string Name,
    string Sku,
    Dictionary<string, string> Attributes,
    int StockQuantity,
    bool IsActive);

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    string Slug,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive);

public record ProductsResponse(
    List<ProductResponse> Products,
    int TotalCount,
    int Page,
    int PageSize);
