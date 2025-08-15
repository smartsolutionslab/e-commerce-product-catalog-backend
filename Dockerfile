FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/E-Commerce.ProductCatalog.Api/E-Commerce.ProductCatalog.Api.csproj", "src/E-Commerce.ProductCatalog.Api/"]
COPY ["src/E-Commerce.ProductCatalog.Application/E-Commerce.ProductCatalog.Application.csproj", "src/E-Commerce.ProductCatalog.Application/"]
COPY ["src/E-Commerce.ProductCatalog.Domain/E-Commerce.ProductCatalog.Domain.csproj", "src/E-Commerce.ProductCatalog.Domain/"]
COPY ["src/E-Commerce.ProductCatalog.Infrastructure/E-Commerce.ProductCatalog.Infrastructure.csproj", "src/E-Commerce.ProductCatalog.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/E-Commerce.ProductCatalog.Api/E-Commerce.ProductCatalog.Api.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/src/E-Commerce.ProductCatalog.Api"
RUN dotnet build "E-Commerce.ProductCatalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "E-Commerce.ProductCatalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Commerce.ProductCatalog.Api.dll"]
