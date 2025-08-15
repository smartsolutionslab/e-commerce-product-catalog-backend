using E_Commerce.Common.Application.Abstractions;
using E_Commerce.ProductCatalog.Application.DTOs;
using E_Commerce.ProductCatalog.Application.Interfaces;
using E_Commerce.ProductCatalog.Application.Queries;
using MediatR;

namespace E_Commerce.ProductCatalog.Application.Handlers;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<ProductsResponse>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductsResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetPagedAsync(
                request.Page, 
                request.PageSize, 
                request.SearchTerm, 
                request.CategoryId, 
                cancellationToken);

            var totalCount = await _productRepository.GetTotalCountAsync(
                request.SearchTerm, 
                request.CategoryId, 
                cancellationToken);

            var productResponses = products.Select(p => new ProductResponse(
                p.Id.Value,
                p.Name,
                p.Description,
                p.Sku,
                p.CategoryId.Value,
                p.Status.ToString(),
                p.StockQuantity,
                p.MinStockLevel,
                p.Prices.Select(price => new ProductPriceResponse(
                    price.Id.Value,
                    price.Amount,
                    price.Currency,
                    price.EffectiveFrom,
                    price.EffectiveTo
                )).ToList(),
                p.Variants.Select(variant => new ProductVariantResponse(
                    variant.Id.Value,
                    variant.Name,
                    variant.Sku,
                    variant.Attributes,
                    variant.StockQuantity,
                    variant.IsActive
                )).ToList(),
                p.CreatedAt
            )).ToList();

            return Result.Success(new ProductsResponse(productResponses, totalCount, request.Page, request.PageSize));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProductsResponse>(new Error("GetProducts.Failed", ex.Message));
        }
    }
}
