using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.ValueObjects;

namespace E_Commerce.ProductCatalog.Domain.Events;

public record ProductCreatedEvent(
    ProductId ProductId,
    TenantId TenantId,
    string Name,
    string Sku,
    CategoryId CategoryId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductUpdatedEvent(
    ProductId ProductId,
    TenantId TenantId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductDeactivatedEvent(
    ProductId ProductId,
    TenantId TenantId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductActivatedEvent(
    ProductId ProductId,
    TenantId TenantId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductPriceChangedEvent(
    ProductId ProductId,
    TenantId TenantId,
    decimal Amount,
    string Currency) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductInventoryUpdatedEvent(
    ProductId ProductId,
    TenantId TenantId,
    int OldQuantity,
    int NewQuantity) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductLowStockEvent(
    ProductId ProductId,
    TenantId TenantId,
    int CurrentQuantity,
    int MinStockLevel) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductVariantAddedEvent(
    ProductId ProductId,
    TenantId TenantId,
    ProductVariantId VariantId,
    string VariantName,
    string VariantSku) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CategoryCreatedEvent(
    CategoryId CategoryId,
    TenantId TenantId,
    string Name,
    CategoryId? ParentCategoryId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CategoryUpdatedEvent(
    CategoryId CategoryId,
    TenantId TenantId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
