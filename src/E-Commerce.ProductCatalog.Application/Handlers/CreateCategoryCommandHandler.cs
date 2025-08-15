using E_Commerce.Common.Application.Abstractions;
using E_Commerce.ProductCatalog.Application.Commands;
using E_Commerce.ProductCatalog.Application.Interfaces;
using E_Commerce.ProductCatalog.Domain.Entities;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.ProductCatalog.Application.Handlers;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryId>>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryId>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            CategoryId? parentCategoryId = null;
            
            if (request.ParentCategoryId.HasValue)
            {
                parentCategoryId = CategoryId.Create(request.ParentCategoryId.Value);
                var parentCategory = await _categoryRepository.GetByIdAsync(parentCategoryId, cancellationToken);
                if (parentCategory == null)
                    return Result.Failure<CategoryId>(new Error("ParentCategory.NotFound", "Parent category not found"));
            }

            var category = Category.Create(request.TenantId, request.Name, request.Description, parentCategoryId);

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success(category.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<CategoryId>(new Error("CreateCategory.Failed", ex.Message));
        }
    }
}
