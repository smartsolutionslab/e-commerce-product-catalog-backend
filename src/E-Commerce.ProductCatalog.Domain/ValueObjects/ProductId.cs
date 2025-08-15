namespace E_Commerce.ProductCatalog.Domain.ValueObjects;

public record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static ProductId Create(Guid value) => new(value);
    public static ProductId Create(string value) => new(Guid.Parse(value));
    public static ProductId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(ProductId productId) => productId.Value;
    public static implicit operator string(ProductId productId) => productId.Value.ToString();

    public override string ToString() => Value.ToString();
}

public record CategoryId
{
    public Guid Value { get; }

    private CategoryId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static CategoryId Create(Guid value) => new(value);
    public static CategoryId Create(string value) => new(Guid.Parse(value));
    public static CategoryId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(CategoryId categoryId) => categoryId.Value;
    public override string ToString() => Value.ToString();
}
