using E_Commerce.Common.Application.Abstractions;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.ValueObjects;

namespace E_Commerce.ProductCatalog.Application.Commands;

public record CreateProductCommand(
    TenantId TenantId,
    string Name,
    string Description,
    string Sku,
    Guid CategoryId,
    decimal Price,
    string Currency,
    int StockQuantity) : ICommand<ProductId>;

public record UpdateProductCommand(
    ProductId ProductId,
    string Name,
    string Description,
    Guid CategoryId) : ICommand;

public record UpdateProductPriceCommand(
    ProductId ProductId,
    decimal Price,
    string Currency) : ICommand;

public record UpdateProductInventoryCommand(
    ProductId ProductId,
    int Quantity) : ICommand;

public record DeactivateProductCommand(ProductId ProductId) : ICommand;

public record CreateCategoryCommand(
    TenantId TenantId,
    string Name,
    string Description,
    Guid? ParentCategoryId) : ICommand<CategoryId>;

public record UpdateCategoryCommand(
    CategoryId CategoryId,
    string Name,
    string Description) : ICommand;
