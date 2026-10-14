# StockPilot — Inventory Management System

**StockPilot** is a full-stack inventory management application designed to help businesses manage products, monitor stock levels, track inventory movements, and organize suppliers.

The project is being developed using **ASP.NET Core, Entity Framework Core, Microsoft SQL Server, and Angular**, with a focus on clean architecture, maintainability, automated testing, and professional software engineering practices.

> **Project status:** In active development. The backend currently focuses on product management, including CRUD operations, pagination, search, and initial unit tests.

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Backend Project Structure](#backend-project-structure)
- [Implemented Features](#implemented-features)
- [Project Roadmap](#project-roadmap)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Database Configuration](#database-configuration)
- [Running the Application](#running-the-application)
- [Testing](#testing)
- [Development Approach](#development-approach)

## Overview

StockPilot aims to provide a centralized platform for inventory management, enabling users to manage products, monitor stock quantities, organize suppliers, and track inventory operations.

The application is being developed incrementally, starting with a structured backend before expanding into additional business modules and the Angular frontend.

The main objectives are to:

- Build a maintainable REST API using ASP.NET Core.
- Apply a layered architecture with clear separation of responsibilities.
- Persist application data using SQL Server and Entity Framework Core.
- Implement inventory-related business rules.
- Develop automated tests to improve reliability.
- Integrate the backend with an Angular frontend.
- Introduce CI/CD automation and deployment practices.

## Technology Stack

### Backend

- **C#** — Main programming language.
- **.NET 10** — Application platform.
- **ASP.NET Core Web API** — REST API development.
- **Entity Framework Core** — Object-relational mapping (ORM).
- **Microsoft SQL Server** — Relational database.
- **Swagger / OpenAPI** — API documentation and interactive testing.
- **xUnit** — Unit testing framework.

### Tools and Development

- Visual Studio
- SQL Server Management Studio (SSMS)
- Git and GitHub

### Planned Frontend

- Angular
- TypeScript
- HTML and CSS
- Integration with the StockPilot REST API

## Architecture

The backend follows a layered architecture that separates the API, application logic, domain model, and infrastructure concerns.

```text
Client / Frontend
       |
       v
StockPilot.API
       |
       v
StockPilot.Application
       |
       +------------------+
       |                  |
       v                  v
StockPilot.Domain   Repository Interfaces
                          ^
                          |
                 StockPilot.Infrastructure
                          |
                          v
                    SQL Server
```

### Layer Responsibilities

#### StockPilot.API

The presentation layer responsible for exposing HTTP endpoints, receiving requests, validating incoming request models, and returning HTTP responses.

#### StockPilot.Application

Contains application services, business rules, DTOs, and interfaces that define how application logic accesses data.

#### StockPilot.Domain

Contains the core domain entities and business concepts, independent of database implementation details.

#### StockPilot.Infrastructure

Implements persistence and data-access operations using Entity Framework Core and SQL Server.

#### StockPilot.Tests

Contains automated tests for application behavior, including product creation, updates, deletion, pagination, and search.

## Backend Project Structure

```text
StockPilotBackEnd/
├── StockPilot.API/
│   ├── Controllers/
│   │   └── ProductsController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── ...
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
│   ├── Repositories/
│   │   └── ProductRepository.cs
│   └── ...
│
├── StockPilot.Tests/
│   └── ProductServiceTests.cs
│
└── StockPilotBackEnd.sln
```

*Note: This structure reflects the current organization of the solution. Additional folders and modules will be introduced as the project evolves.*

## Implemented Features

### 1. Product Management

The current backend provides the main CRUD operations for products:

- Create a product.
- Retrieve products.
- Retrieve a product by ID.
- Update an existing product.
- Delete a product.

Each product currently contains fields such as:

- ID
- Name
- SKU (Stock Keeping Unit)
- Description
- Unit price
- Quantity in stock
- Minimum stock level
- Creation timestamp

### 2. Pagination and Search

The product listing endpoint supports:

- Pagination using a page number and page size.
- Search by product name, SKU, or description.
- Total result count.
- Total number of pages.
- A configurable page-size limit.

Pagination is implemented at the repository level using Entity Framework Core, allowing SQL Server to retrieve the requested subset of records instead of loading the entire result set into application memory.

### 3. Validation and Business Rules

The current implementation includes:

- Required-field validation.
- String-length restrictions.
- Numeric range validation.
- Duplicate SKU detection.
- Appropriate HTTP responses for common success and error scenarios.

### 4. Database Integration

- SQL Server database connectivity.
- Entity Framework Core integration.
- Database migrations.
- Asynchronous data-access operations.

### 5. Automated Tests

Initial unit tests cover:

- Successful product creation.
- Duplicate SKU rejection.
- Product updates.
- Handling unknown product IDs.
- Product deletion.
- Pagination and search behavior.

## Project Roadmap

The following modules represent the planned evolution of StockPilot. They are not all implemented yet.

### 1. Product Management

**Status: Initial implementation available**

- Product CRUD operations.
- Search and pagination.
- Product validation.
- Minimum stock levels.

Future improvements include more comprehensive tests and additional product-related business rules.

### 2. Inventory Management

**Planned**

- Track current inventory quantities.
- Record stock entries and exits.
- Manage stock adjustments.
- Maintain an inventory movement history.
- Detect low-stock and out-of-stock products.
- Apply business rules to prevent invalid stock operations.

### 3. Category Management

**Planned**

- Create, update, and delete categories.
- Organize products by category.
- Define relationships between categories and products.

### 4. Supplier Management

**Planned**

- Manage supplier information.
- Associate suppliers with products.
- Store supplier contact details.
- Prepare supplier data for replenishment workflows.

### 5. Stock Movements and Purchase Orders

**Planned**

- Record incoming and outgoing inventory movements.
- Track changes to stock quantities.
- Manage purchase orders.
- Record goods received from suppliers.
- Preserve a traceable history of inventory operations.

### 6. Dashboard and Reporting

**Planned**

- Display key inventory statistics.
- Identify low-stock and out-of-stock products.
- Provide an overview of inventory quantities.
- Explore inventory valuation and reporting features.

### 7. Authentication and Authorization

**Planned**

- Implement secure user authentication.
- Introduce role-based access control.
- Define roles such as Administrator and Employee.
- Restrict operations according to user permissions.

### 8. Testing and Code Quality

**Planned improvements**

- Expand unit test coverage.
- Add API integration tests.
- Improve centralized exception handling.
- Introduce structured logging.
- Validate important inventory business rules.

### 9. CI/CD and Deployment

**Planned**

- Automate builds and tests.
- Configure a continuous integration pipeline.
- Introduce deployment automation.
- Deploy the backend to a cloud platform.

### 10. Angular Frontend

**Planned**

Develop an Angular frontend that communicates with the backend API and provides interfaces for product management, inventory operations, suppliers, reporting, and user administration.

## API Endpoints

Base route:

```text
/api/products
```

| HTTP Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products` | Retrieve a paginated product list |
| GET | `/api/products/{id}` | Retrieve a product by ID |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Pagination Examples

Retrieve the first page with 10 products:

```http
GET /api/products?page=1&pageSize=10
```

Retrieve the second page:

```http
GET /api/products?page=2&pageSize=10
```

Search for products containing `Logitech`:

```http
GET /api/products?search=Logitech
```

Combine pagination and search:

```http
GET /api/products?page=1&pageSize=5&search=Logitech
```

### Example Paginated Response

```json
{
  "items": [
    {
      "id": 1,
      "name": "Clavier Logitech K120",
      "sku": "CLAV-001",
      "description": "Clavier USB filaire",
      "unitPrice": 19.99,
      "quantityInStock": 35,
      "minimumStockLevel": 5
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 20,
  "totalPages": 2
}
```

The `items` array contains the products returned for the current page. The remaining fields provide pagination metadata.

## Getting Started

### Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server)
- [SQL Server Management Studio](https://learn.microsoft.com/en-us/ssms/install/install)
- [Visual Studio](https://visualstudio.microsoft.com/) or another compatible .NET IDE

### 1. Clone the Repository

Replace the placeholder with your GitHub repository URL.

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
cd StockPilotBackEnd
```

### 2. Configure the Database Connection

Create a SQL Server database named `StockPilotDb`.

Update the `DefaultConnection` entry in `StockPilot.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=StockPilotDb;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

Replace `YOUR_SERVER` with your SQL Server instance name.

The example uses Windows authentication. Configure the connection string according to your environment.

**Security note:** Do not commit production credentials or sensitive connection details to a public repository. Use environment variables, user secrets, or a managed secrets provider where appropriate.

### 3. Restore Dependencies

From the solution directory, run:

```bash
dotnet restore
```

### 4. Apply Database Migrations

The project uses Entity Framework Core migrations to create and update the database schema.

If the Entity Framework Core CLI is not installed, install it:

```bash
dotnet tool install --global dotnet-ef
```

Then apply the existing migrations:

```bash
dotnet ef database update --project StockPilot.Infrastructure --startup-project StockPilot.API
```

If the repository's migration configuration changes, consult the current solution setup before running this command.

### 5. Build the Solution

```bash
dotnet build
```

### 6. Run the API

```bash
dotnet run --project StockPilot.API
```

The application will print its listening URLs in the terminal.

In the Development environment, Swagger is available at the configured Swagger URL, for example:

```text
https://localhost:7253/swagger
```

Use the actual URL printed by the application if the port differs.

## Testing

Run the automated tests from the solution directory:

```bash
dotnet test
```

To build and test the solution before committing changes:

```bash
dotnet build
dotnet test
```

## Development Approach

StockPilot is being developed incrementally:

1. Establish the backend architecture and product management foundation.
2. Extend the domain model with categories, suppliers, and inventory concepts.
3. Implement stock movement workflows and inventory business rules.
4. Improve validation, error handling, logging, and automated testing.
5. Introduce authentication and authorization.
6. Set up CI/CD and deployment.
7. Develop and integrate the Angular frontend.

The roadmap may evolve as the application grows.

## License

This project is currently intended for learning and portfolio purposes. A license will be added if the project is distributed under specific terms.
