using E_Commerce.Common.Application.Abstractions;
using E_Commerce.ProductCatalog.Application.Commands;
using E_Commerce.ProductCatalog.Application.Interfaces;
using E_Commerce.ProductCatalog.Domain.Entities;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.ProductCatalog.Application.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductId>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<ProductId>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate category exists
            var categoryId = CategoryId.Create(request.CategoryId);
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                return Result.Failure<ProductId>(new Error("Category.NotFound", "Category not found"));

            // Check SKU uniqueness
            var existingProduct = await _productRepository.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingProduct != null)
                return Result.Failure<ProductId>(new Error("Product.SkuExists", "Product with this SKU already exists"));

            var product = Product.Create(request.TenantId, request.Name, request.Description, request.Sku, categoryId);
            
            product.SetPrice(request.Price, request.Currency);
            product.UpdateInventory(request.StockQuantity);

            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            return Result.Success(product.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<ProductId>(new Error("CreateProduct.Failed", ex.Message));
        }
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result.Failure(new Error("Product.NotFound", "Product not found"));

            var categoryId = CategoryId.Create(request.CategoryId);
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
                return Result.Failure(new Error("Category.NotFound", "Category not found"));

            product.UpdateDetails(request.Name, request.Description, categoryId);

            await _productRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UpdateProduct.Failed", ex.Message));
        }
    }
}

public class UpdateProductInventoryCommandHandler : IRequestHandler<UpdateProductInventoryCommand, Result>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductInventoryCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result> Handle(UpdateProductInventoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result.Failure(new Error("Product.NotFound", "Product not found"));

            product.UpdateInventory(request.Quantity);

            await _productRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UpdateInventory.Failed", ex.Message));
        }
    }
}
