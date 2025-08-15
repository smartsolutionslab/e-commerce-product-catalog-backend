using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.Events;

namespace E_Commerce.ProductCatalog.Domain.Entities;

public sealed class Product : Entity<ProductId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Sku { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public ProductStatus Status { get; private set; }
    public int StockQuantity { get; private set; }
    public int MinStockLevel { get; private set; }
    public DateTime? DiscontinuedAt { get; private set; }
    
    private readonly List<ProductPrice> _prices = [];
    public IReadOnlyList<ProductPrice> Prices => _prices.AsReadOnly();
    
    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();

    private Product(ProductId id, TenantId tenantId, string name, string description, string sku, CategoryId categoryId)
        : base(id, tenantId)
    {
        Name = name;
        Description = description;
        Sku = sku;
        CategoryId = categoryId;
        Status = ProductStatus.Active;
        StockQuantity = 0;
        MinStockLevel = 0;
    }

    private Product() : base() { } // For EF

    public static Product Create(TenantId tenantId, string name, string description, string sku, CategoryId categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("Product SKU cannot be empty", nameof(sku));

        var product = new Product(ProductId.NewId(), tenantId, name.Trim(), description?.Trim() ?? "", sku.Trim().ToUpperInvariant(), categoryId);
        
        product.RaiseDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.CategoryId));

        return product;
    }

    public void UpdateDetails(string name, string description, CategoryId categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        var hasChanges = false;

        if (Name != name.Trim())
        {
            Name = name.Trim();
            hasChanges = true;
        }

        if (Description != (description?.Trim() ?? ""))
        {
            Description = description?.Trim() ?? "";
            hasChanges = true;
        }

        if (CategoryId != categoryId)
        {
            CategoryId = categoryId;
            hasChanges = true;
        }

        if (hasChanges)
        {
            MarkAsUpdated();
            RaiseDomainEvent(new ProductUpdatedEvent(Id, TenantId));
        }
    }

    public void SetPrice(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Price cannot be negative", nameof(amount));

        var existingPrice = _prices.FirstOrDefault(p => p.Currency == currency);
        if (existingPrice != null)
        {
            existingPrice.UpdateAmount(amount);
        }
        else
        {
            var price = ProductPrice.Create(amount, currency);
            _prices.Add(price);
        }

        MarkAsUpdated();
        RaiseDomainEvent(new ProductPriceChangedEvent(Id, TenantId, amount, currency));
    }

    public void UpdateInventory(int newQuantity)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(newQuantity));

        var oldQuantity = StockQuantity;
        StockQuantity = newQuantity;
        MarkAsUpdated();

        RaiseDomainEvent(new ProductInventoryUpdatedEvent(Id, TenantId, oldQuantity, newQuantity));

        if (newQuantity <= MinStockLevel && newQuantity > 0)
        {
            RaiseDomainEvent(new ProductLowStockEvent(Id, TenantId, newQuantity, MinStockLevel));
        }
    }

    public void SetMinStockLevel(int minLevel)
    {
        if (minLevel < 0)
            throw new ArgumentException("Minimum stock level cannot be negative", nameof(minLevel));

        MinStockLevel = minLevel;
        MarkAsUpdated();
    }

    public void AddVariant(string name, string sku, Dictionary<string, string> attributes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Variant name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("Variant SKU cannot be empty", nameof(sku));

        if (_variants.Any(v => v.Sku == sku.Trim().ToUpperInvariant()))
            throw new InvalidOperationException($"Variant with SKU {sku} already exists");

        var variant = ProductVariant.Create(name.Trim(), sku.Trim().ToUpperInvariant(), attributes);
        _variants.Add(variant);
        MarkAsUpdated();

        RaiseDomainEvent(new ProductVariantAddedEvent(Id, TenantId, variant.Id, name, sku));
    }

    public void Deactivate()
    {
        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("Product is already discontinued");

        Status = ProductStatus.Discontinued;
        DiscontinuedAt = DateTime.UtcNow;
        MarkAsUpdated();

        RaiseDomainEvent(new ProductDeactivatedEvent(Id, TenantId));
    }

    public void Activate()
    {
        if (Status == ProductStatus.Active)
            return;

        Status = ProductStatus.Active;
        DiscontinuedAt = null;
        MarkAsUpdated();

        RaiseDomainEvent(new ProductActivatedEvent(Id, TenantId));
    }

    public bool IsInStock() => StockQuantity > 0;
    public bool IsLowStock() => StockQuantity <= MinStockLevel && StockQuantity > 0;
    public decimal? GetPrice(string currency) => _prices.FirstOrDefault(p => p.Currency == currency)?.Amount;
}

public enum ProductStatus
{
    Active,
    Discontinued
}
