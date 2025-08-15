using E_Commerce.Common.Api.Extensions;
using E_Commerce.ProductCatalog.Api.Endpoints;
using E_Commerce.ProductCatalog.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Common services
builder.Services.AddCommonServices();
builder.Services.AddMultiTenancy();
builder.Services.AddApiVersioning();

// Product catalog specific services
builder.Services.AddProductCatalogInfrastructure(builder.Configuration);

// Health checks
builder.Services.AddHealthChecks();

// API documentation
builder.Services.AddSwaggerWithVersioning();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
    });
}

app.UseHttpsRedirection();

// Add product endpoints
app.MapProductEndpoints();

// Health check
app.MapHealthChecks("/health");

app.Run();
