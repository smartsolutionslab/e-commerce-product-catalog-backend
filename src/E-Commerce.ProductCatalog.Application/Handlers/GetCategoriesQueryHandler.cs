using E_Commerce.Common.Application.Abstractions;
using E_Commerce.ProductCatalog.Application.DTOs;
using E_Commerce.ProductCatalog.Application.Interfaces;
using E_Commerce.ProductCatalog.Application.Queries;
using MediatR;

namespace E_Commerce.ProductCatalog.Application.Handlers;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryResponse>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);

            var categoryResponses = categories.Select(c => new CategoryResponse(
                c.Id.Value,
                c.Name,
                c.Description,
                c.Slug,
                c.ParentCategoryId?.Value,
                c.SortOrder,
                c.IsActive
            )).ToList();

            return Result.Success(categoryResponses);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<CategoryResponse>>(new Error("GetCategories.Failed", ex.Message));
        }
    }
}
