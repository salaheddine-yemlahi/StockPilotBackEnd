# StockPilot — Backend API

A backend REST API for inventory and product management, built with ASP.NET Core, Entity Framework Core, and Microsoft SQL Server.

## Overview

StockPilot is a full-stack inventory management project. This repository contains the backend, responsible for managing products, validating incoming data, processing business rules, and persisting information in a SQL Server database.

The backend follows a layered architecture to separate business logic, data access, API endpoints, and domain models.

## Tech Stack

- **C# / .NET 10** — Backend development
- **ASP.NET Core Web API** — REST API endpoints
- **Entity Framework Core** — Object-relational mapping (ORM)
- **Microsoft SQL Server** — Relational database
- **Swagger / OpenAPI** — API documentation and testing
- **xUnit** — Unit testing
- **Git / GitHub** — Version control

## Project Structure

```text
StockPilotBackEnd/
├── StockPilot.API/
│   ├── Controllers/
│   │   └── ProductsController.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── StockPilot.Application/
│   └── Products/
│       ├── DTOs/
│       │   ├── CreateProductDto.cs
│       │   ├── UpdateProductDto.cs
│       │   ├── ProductDto.cs
│       │   └── PagedResultDto.cs
│       ├── Interfaces/
│       │   ├── IProductRepository.cs
│       │   └── IProductService.cs
│       └── ProductService.cs
│
├── StockPilot.Domain/
│   └── Entities/
│       └── Product.cs
│
├── StockPilot.Infrastructure/
│   ├── Persistence/
│   │   └── StockPilotDbContext.cs
│   └── Repositories/
│       └── ProductRepository.cs
│
└── StockPilot.Tests/
    └── ProductServiceTests.cs
```

### Architecture

- **StockPilot.API:** Exposes HTTP endpoints, receives requests, and returns appropriate HTTP responses.
- **StockPilot.Application:** Contains application services, business rules, DTOs, and repository interfaces.
- **StockPilot.Domain:** Defines the core domain entities, including the `Product` entity.
- **StockPilot.Infrastructure:** Implements database access using Entity Framework Core and SQL Server.
- **StockPilot.Tests:** Contains unit tests for application services and product management operations.

## Features

### Product Management (CRUD)

The API supports the following operations:

- Create a product.
- Retrieve all products.
- Retrieve a product by its ID.
- Update an existing product.
- Delete a product.

### Pagination and Search

The product listing endpoint supports:

- Pagination with configurable page number and page size.
- Search by product name, SKU, or description.
- Pagination metadata, including total results and total pages.
- Page-size limits to prevent excessively large responses.

### Validation and Error Handling

- Required-field and value-range validation using Data Annotations.
- Duplicate SKU detection.
- Appropriate HTTP status codes for successful operations, missing products, and duplicate SKUs.

### Database Integration

- SQL Server integration through Entity Framework Core.
- Database migrations for schema management.
- Asynchronous database operations.

### Testing

- Unit tests for product creation, updating, and deletion.
- Tests for duplicate SKU handling.
- Tests for pagination and search.

## API Endpoints

Base route: `/api/products`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products` | Retrieve a paginated list of products |
| GET | `/api/products/{id}` | Retrieve a product by ID |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Pagination and Search Examples

Retrieve the first page with 10 products:

```http
GET /api/products?page=1&pageSize=10
```

Retrieve the second page:

```http
GET /api/products?page=2&pageSize=10
```

Search for products containing "Logitech":

```http
GET /api/products?search=Logitech
```

Combine pagination and search:

```http
GET /api/products?page=1&pageSize=5&search=Logitech
```

A paginated response contains the following fields:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 10,
  "totalCount": 20,
  "totalPages": 2
}
```

The `items` array contains the products matching the requested page and search criteria.

## Getting Started

### Prerequisites

Make sure you have installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/ssms/install/install)
- [Visual Studio](https://visualstudio.microsoft.com/) or another compatible .NET IDE

### 1. Clone the Repository

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
cd StockPilotBackEnd
```

### 2. Configure the Database

Create a SQL Server database named `StockPilotDb`.

Update the connection string in `StockPilot.API/appsettings.json` to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=StockPilotDb;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

Replace `YOUR_SERVER` with your SQL Server instance name.

For production environments, use secure configuration mechanisms instead of committing credentials or environment-specific secrets.

### 3. Apply Database Migrations

From the solution directory, run:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project StockPilot.Infrastructure --startup-project StockPilot.API
```

If `dotnet-ef` is already installed, update it if necessary instead of installing it again.

### 4. Run the API

```bash
dotnet run --project StockPilot.API
```

The application will display its local URLs in the console.

In the Development environment, Swagger is available at the configured Swagger URL, for example:

```text
https://localhost:7253/swagger
```

Use the actual URL printed by your application if the port differs.

## Running Tests

Run the unit tests from the solution directory:

```bash
dotnet test
```

## Future Improvements

Planned enhancements may include:

- Angular frontend integration.
- More comprehensive API integration tests.
- Centralized exception handling.
- Structured logging.
- Authentication and authorization.
- Continuous Integration and Continuous Deployment (CI/CD).
- Automated deployment to a cloud platform.

## License

This project is currently intended for learning and portfolio purposes. Add a license if you decide to distribute it under specific terms.
