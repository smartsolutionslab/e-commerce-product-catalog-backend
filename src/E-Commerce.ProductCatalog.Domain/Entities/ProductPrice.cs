using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;

namespace E_Commerce.ProductCatalog.Domain.Entities;

public class ProductPrice : Entity<ProductPriceId>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    private ProductPrice(ProductPriceId id, TenantId tenantId, decimal amount, string currency)
        : base(id, tenantId)
    {
        Amount = amount;
        Currency = currency;
        EffectiveFrom = DateTime.UtcNow;
    }

    private ProductPrice() : base() { } // For EF

    public static ProductPrice Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Price amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        return new ProductPrice(
            ProductPriceId.NewId(),
            TenantId.NewId(), // Will be set by Product aggregate
            Math.Round(amount, 2),
            currency.ToUpperInvariant());
    }

    public void UpdateAmount(decimal newAmount)
    {
        if (newAmount < 0)
            throw new ArgumentException("Price amount cannot be negative", nameof(newAmount));

        Amount = Math.Round(newAmount, 2);
        MarkAsUpdated();
    }

    public void SetEffectivePeriod(DateTime from, DateTime? to = null)
    {
        if (to.HasValue && to.Value <= from)
            throw new ArgumentException("Effective to date must be after from date");

        EffectiveFrom = from;
        EffectiveTo = to;
        MarkAsUpdated();
    }

    public bool IsEffective(DateTime date) =>
        date >= EffectiveFrom && (EffectiveTo == null || date <= EffectiveTo);
}

public record ProductPriceId
{
    public Guid Value { get; }

    private ProductPriceId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductPriceId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static ProductPriceId Create(Guid value) => new(value);
    public static ProductPriceId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(ProductPriceId id) => id.Value;
    public override string ToString() => Value.ToString();
}
