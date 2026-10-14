## Overview

**StockPilot** is a full-stack inventory management application designed to help businesses manage products, monitor stock levels, track inventory movements, and organize suppliers.

The project is being developed with a modern architecture, using ASP.NET Core for the backend, Microsoft SQL Server for data persistence, and Angular for the frontend.

The goal is to build a maintainable, secure, and testable application while applying professional software development practices.

## Project Roadmap

StockPilot is being developed incrementally, with the following planned modules:

### 1. Product Management
- Create, update, delete, and retrieve products.
- Search products by name, SKU, and description.
- Paginated product listings.
- Validate product information and prevent duplicate SKUs.
- Configure minimum stock levels.

### 2. Inventory Management
- Track current stock quantities.
- Record stock entries and exits.
- Manage stock adjustments.
- Maintain an inventory movement history.
- Identify products with low stock or insufficient quantities.

### 3. Category Management
- Organize products into categories.
- Manage category information.
- Associate products with their respective categories.

### 4. Supplier Management
- Manage supplier information.
- Associate suppliers with products.
- Prepare the foundation for purchase and replenishment workflows.

### 5. Stock Movements and Orders
- Record incoming and outgoing stock movements.
- Track inventory changes.
- Plan purchase order management and goods reception.

### 6. Dashboard and Reporting
- Display key inventory statistics.
- Identify low-stock and out-of-stock products.
- Provide an overview of inventory levels.
- Explore stock valuation and reporting.

### 7. Authentication and Authorization
- Implement secure user authentication.
- Introduce role-based access control.
- Define permissions for roles such as Administrator and Employee.

### 8. Engineering and Quality
- Unit and integration testing.
- Centralized exception handling and structured logging.
- CI/CD automation.
- Deployment to a cloud platform.

## Current Implementation Status

The project is being developed progressively. The current backend implementation focuses on the product management module.

**Implemented:**
- ASP.NET Core Web API project structure.
- Layered architecture separating API, Application, Domain, and Infrastructure.
- SQL Server integration using Entity Framework Core.
- Database migrations.
- Product CRUD endpoints.
- Product validation and duplicate SKU detection.
- Pagination and search by product name, SKU, or description.
- Initial unit tests for product service operations, pagination, and search.
- Swagger/OpenAPI documentation for API exploration.

**Planned:**
- Categories and supplier management.
- Inventory movement tracking and stock business rules.
- Dashboard and reporting capabilities.
- Authentication and authorization.
- More comprehensive automated testing.
- CI/CD and deployment.
- Angular frontend integration.

The implementation status will be updated as development progresses.

## Development Approach

The project follows an incremental development approach:

1. Establish the backend architecture and product management foundation.
2. Extend the domain model and implement inventory business rules.
3. Improve testing, validation, error handling, and security.
4. Automate build and testing workflows using CI/CD.
5. Develop and integrate the Angular frontend.
6. Deploy the application and document its operation.
