using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;

namespace E_Commerce.ProductCatalog.Domain.Entities;

public class ProductVariant : Entity<ProductVariantId>
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public Dictionary<string, string> Attributes { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    private ProductVariant(ProductVariantId id, TenantId tenantId, string name, string sku, Dictionary<string, string> attributes)
        : base(id, tenantId)
    {
        Name = name;
        Sku = sku;
        Attributes = attributes ?? new Dictionary<string, string>();
        StockQuantity = 0;
        IsActive = true;
    }

    private ProductVariant() : base() { } // For EF

    public static ProductVariant Create(string name, string sku, Dictionary<string, string> attributes)
    {
        return new ProductVariant(ProductVariantId.NewId(), TenantId.NewId(), name, sku, attributes);
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

        StockQuantity = quantity;
        MarkAsUpdated();
    }

    public void UpdateAttributes(Dictionary<string, string> attributes)
    {
        Attributes = attributes ?? new Dictionary<string, string>();
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
}

public record ProductVariantId
{
    public Guid Value { get; }

    private ProductVariantId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductVariantId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static ProductVariantId Create(Guid value) => new(value);
    public static ProductVariantId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(ProductVariantId id) => id.Value;
    public override string ToString() => Value.ToString();
}
