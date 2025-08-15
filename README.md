# E-Commerce Product Catalog Backend

Microservice for product and category management with Clean Architecture, CQRS, and Event Sourcing.

## Features

- Product CRUD Operations with Variants
- Category Hierarchy Management
- Inventory Tracking
- Price Management with Multiple Currencies
- Product Search and Filtering
- Multi-Tenancy Support
- Event Sourcing
- RESTful API with Versioning

## API Endpoints

### v1 Endpoints
- `GET /api/v1/products` - List products with filtering
- `GET /api/v1/products/{id}` - Get product by ID
- `POST /api/v1/products` - Create product
- `PUT /api/v1/products/{id}` - Update product
- `DELETE /api/v1/products/{id}` - Delete product
- `GET /api/v1/categories` - List categories
- `POST /api/v1/categories` - Create category

## Domain Events

- ProductCreated, ProductUpdated, ProductDeactivated
- InventoryUpdated, PriceChanged
- CategoryCreated, CategoryUpdated

## Run Locally

```bash
dotnet run
```

Access: https://localhost:7002
