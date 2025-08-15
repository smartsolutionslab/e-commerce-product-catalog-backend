using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.Events;

namespace E_Commerce.ProductCatalog.Domain.Entities;

public sealed class Category : Entity<CategoryId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Slug { get; private set; }
    public CategoryId? ParentCategoryId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    private Category(CategoryId id, TenantId tenantId, string name, string description, string slug, CategoryId? parentCategoryId)
        : base(id, tenantId)
    {
        Name = name;
        Description = description;
        Slug = slug;
        ParentCategoryId = parentCategoryId;
        SortOrder = 0;
        IsActive = true;
    }

    private Category() : base() { } // For EF

    public static Category Create(TenantId tenantId, string name, string description, CategoryId? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        var slug = GenerateSlug(name);
        var category = new Category(CategoryId.NewId(), tenantId, name.Trim(), description?.Trim() ?? "", slug, parentCategoryId);
        
        category.RaiseDomainEvent(new CategoryCreatedEvent(
            category.Id,
            category.TenantId,
            category.Name,
            category.ParentCategoryId));

        return category;
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim() ?? "";
        Slug = GenerateSlug(name);
        MarkAsUpdated();

        RaiseDomainEvent(new CategoryUpdatedEvent(Id, TenantId));
    }

    public void SetParent(CategoryId? parentCategoryId)
    {
        if (parentCategoryId?.Value == Id.Value)
            throw new InvalidOperationException("Category cannot be its own parent");

        ParentCategoryId = parentCategoryId;
        MarkAsUpdated();
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
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

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "");
    }
}
