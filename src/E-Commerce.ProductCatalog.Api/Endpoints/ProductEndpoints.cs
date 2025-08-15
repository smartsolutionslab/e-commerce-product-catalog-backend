using Asp.Versioning;
using E_Commerce.ProductCatalog.Application.Commands;
using E_Commerce.ProductCatalog.Application.Queries;
using E_Commerce.ProductCatalog.Application.DTOs;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.ProductCatalog.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ProductCatalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/products")
            .WithApiVersionSet()
            .HasApiVersion(1, 0)
            .WithTags("Products")
            .RequireAuthorization();

        // GET /api/v1/products
        group.MapGet("/", GetProductsAsync)
            .WithName("GetProducts")
            .WithOpenApi();

        // GET /api/v1/products/{id}
        group.MapGet("/{id:guid}", GetProductByIdAsync)
            .WithName("GetProductById")
            .WithOpenApi();

        // POST /api/v1/products
        group.MapPost("/", CreateProductAsync)
            .WithName("CreateProduct")
            .WithOpenApi();

        // PUT /api/v1/products/{id}
        group.MapPut("/{id:guid}", UpdateProductAsync)
            .WithName("UpdateProduct")
            .WithOpenApi();

        // PUT /api/v1/products/{id}/price
        group.MapPut("/{id:guid}/price", UpdateProductPriceAsync)
            .WithName("UpdateProductPrice")
            .WithOpenApi();

        // PUT /api/v1/products/{id}/inventory
        group.MapPut("/{id:guid}/inventory", UpdateProductInventoryAsync)
            .WithName("UpdateProductInventory")
            .WithOpenApi();

        // Categories
        var categoryGroup = app.MapGroup("/api/v{version:apiVersion}/categories")
            .WithApiVersionSet()
            .HasApiVersion(1, 0)
            .WithTags("Categories")
            .RequireAuthorization();

        categoryGroup.MapGet("/", GetCategoriesAsync)
            .WithName("GetCategories")
            .WithOpenApi();

        categoryGroup.MapPost("/", CreateCategoryAsync)
            .WithName("CreateCategory")
            .WithOpenApi();
    }

    private static async Task<IResult> GetProductsAsync(
        [FromServices] IMediator mediator,
        HttpContext context,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null)
    {
        var tenantId = GetTenantId(context);
        if (tenantId == null)
            return Results.BadRequest("Tenant ID is required");

        var query = new GetProductsQuery(
            tenantId,
            page,
            pageSize,
            search,
            categoryId.HasValue ? CategoryId.Create(categoryId.Value) : null);

        var result = await mediator.Send(query);

        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateProductAsync(
        [FromServices] IMediator mediator,
        [FromBody] CreateProductRequest request,
        HttpContext context)
    {
        var tenantId = GetTenantId(context);
        if (tenantId == null)
            return Results.BadRequest("Tenant ID is required");

        var command = new CreateProductCommand(
            tenantId,
            request.Name,
            request.Description,
            request.Sku,
            request.CategoryId,
            request.Price,
            request.Currency,
            request.StockQuantity);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.Created($"/api/v1/products/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetProductByIdAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await mediator.Send(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> UpdateProductAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(
            ProductId.Create(id),
            request.Name,
            request.Description,
            request.CategoryId);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateProductPriceAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] UpdatePriceRequest request)
    {
        var command = new UpdateProductPriceCommand(
            ProductId.Create(id),
            request.Price,
            request.Currency);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateProductInventoryAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] UpdateInventoryRequest request)
    {
        var command = new UpdateProductInventoryCommand(
            ProductId.Create(id),
            request.Quantity);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetCategoriesAsync(
        [FromServices] IMediator mediator,
        HttpContext context)
    {
        var tenantId = GetTenantId(context);
        if (tenantId == null)
            return Results.BadRequest("Tenant ID is required");

        var query = new GetCategoriesQuery(tenantId);
        var result = await mediator.Send(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateCategoryAsync(
        [FromServices] IMediator mediator,
        [FromBody] CreateCategoryRequest request,
        HttpContext context)
    {
        var tenantId = GetTenantId(context);
        if (tenantId == null)
            return Results.BadRequest("Tenant ID is required");

        var command = new CreateCategoryCommand(
            tenantId,
            request.Name,
            request.Description,
            request.ParentCategoryId);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.Created($"/api/v1/categories/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static TenantId? GetTenantId(HttpContext context)
    {
        var tenantIdHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        return string.IsNullOrEmpty(tenantIdHeader) ? null : TenantId.Create(tenantIdHeader);
    }
}

public record CreateProductRequest(
    string Name,
    string Description,
    string Sku,
    Guid CategoryId,
    decimal Price,
    string Currency,
    int StockQuantity);

public record UpdateProductRequest(string Name, string Description, Guid CategoryId);
public record UpdatePriceRequest(decimal Price, string Currency);
public record UpdateInventoryRequest(int Quantity);
public record CreateCategoryRequest(string Name, string Description, Guid? ParentCategoryId);