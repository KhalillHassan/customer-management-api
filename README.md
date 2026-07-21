# Customer Management API

A secure ASP.NET Core Web API for managing users, customers, products, and orders.

The project demonstrates layered architecture, PostgreSQL integration, Entity Framework Core, Dapper reporting, JWT authentication, role-based authorization, validation, logging, API versioning, unit testing, Swagger documentation, and Postman testing.

## Features

- User creation and JWT login
- Password hashing
- Role-based authorization
- Customer CRUD operations
- Product CRUD operations
- Order creation and management
- Automatic stock reduction
- Order status updates
- Customer order summary reports
- Order searching and filtering
- FluentValidation
- Swagger/OpenAPI documentation
- Serilog logging
- API rate limiting
- xUnit and Moq unit tests
- Postman API collection

## Technologies

- .NET 10
- ASP.NET Core Web API
- C#
- PostgreSQL
- Entity Framework Core
- Dapper
- JWT Bearer Authentication
- FluentValidation
- Serilog
- Swagger/OpenAPI
- xUnit
- Moq
- Postman
- Git and GitHub

## Architecture

The solution follows a layered architecture:

```text
Client
  │
  ▼
CustomerManagement.Api
  │
  ▼
CustomerManagement.Business
  │
  ▼
CustomerManagement.Persistence
  │
  ▼
PostgreSQL
```

### Projects

```text
CustomerManagement.Api
├── Controllers
├── Authentication and authorization
├── Swagger configuration
└── Application startup

CustomerManagement.Business
├── Services
├── DTOs
├── Validators
└── Business rules

CustomerManagement.Persistence
├── Entity Framework Core
├── Repositories
├── Unit of Work
├── Database transactions
└── Dapper reports

CustomerManagement.Domain
├── Entities
├── Repository interfaces
└── Report models

CustomerManagement.Tests
└── Business-layer unit tests
```

## Prerequisites

Install:

- .NET 10 SDK
- PostgreSQL
- Git
- Visual Studio, VS Code, or Rider
- Postman, optional
- pgAdmin, optional

Check the installed .NET version:

```bash
dotnet --version
```

## Configuration

Configure the database and JWT settings in:

```text
CustomerManagement.Api/appsettings.Development.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CustomerManagementDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_LONG_RANDOM_SECRET_KEY",
    "Issuer": "CustomerManagementApi",
    "Audience": "CustomerManagementApiUsers",
    "ExpirationMinutes": 60
  }
}
```

Do not commit real passwords, JWT secret keys, or access tokens.

For safer local development, use .NET User Secrets:

```bash
cd CustomerManagement.Api

dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=CustomerManagementDb;Username=postgres;Password=YOUR_PASSWORD"

dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_LONG_RANDOM_SECRET_KEY"
```

## Database Setup

Create the PostgreSQL database:

```sql
CREATE DATABASE "CustomerManagementDb";
```

Restore packages:

```bash
dotnet restore
```

Apply Entity Framework Core migrations:

```powershell
dotnet ef database update --project CustomerManagement.Persistence --startup-project CustomerManagement.Api
```

Run the reporting SQL script using pgAdmin or `psql`:

```bash
psql -U postgres -d CustomerManagementDb -f SqlScripts/ReportingFunctions.sql
```

## Running the API

From the solution root:

```bash
dotnet run --project CustomerManagement.Api
```

For automatic rebuilding:

```bash
dotnet watch --project CustomerManagement.Api
```

The terminal will display the local API URL.

Example:

```text
https://localhost:7001
```

## Authentication

The API uses JWT Bearer Authentication.

### Login

```http
POST /api/v1/auth/login
```

Example body:

```json
{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```

The response contains a JWT:

```json
{
  "token": "JWT_ACCESS_TOKEN"
}
```

Send the token with protected requests:

```http
Authorization: Bearer JWT_ACCESS_TOKEN
```

### Authorization responses

```text
401 Unauthorized
```

The token is missing, invalid, or expired.

```text
403 Forbidden
```

The token is valid, but the user does not have the required role.

## API Endpoints

All versioned endpoints begin with:

```text
/api/v1
```

### Authentication and Users

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/v1/auth/login` | Log in and receive a JWT |
| POST | `/api/v1/users` | Create a user |

### Customers

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/customers` | Get all customers |
| GET | `/api/v1/customers/{id}` | Get a customer by ID |
| POST | `/api/v1/customers` | Create a customer |
| PUT | `/api/v1/customers/{id}` | Update a customer |
| DELETE | `/api/v1/customers/{id}` | Delete a customer |

### Products

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/products` | Get all products |
| GET | `/api/v1/products/{id}` | Get a product by ID |
| POST | `/api/v1/products` | Create a product |
| PUT | `/api/v1/products/{id}` | Update a product |
| DELETE | `/api/v1/products/{id}` | Delete a product |

### Orders

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/orders` | Get all orders |
| GET | `/api/v1/orders/{id}` | Get an order by ID |
| POST | `/api/v1/orders` | Create an order |
| PUT | `/api/v1/orders/{id}/status` | Update order status |
| DELETE | `/api/v1/orders/{id}` | Delete an order |

### Reports

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/order-reports/customers/{customerId}/summary` | Get a customer order summary |
| GET | `/api/v1/order-reports/search` | Search and filter orders |

Example:

```http
GET /api/v1/order-reports/search?customerId=1&status=Pending
```

## Swagger

Start the API and open:

```text
https://localhost:{PORT}/swagger
```

Swagger provides:

- Endpoint documentation
- Request and response models
- HTTP status codes
- Interactive API testing
- JWT authentication support

To test protected endpoints:

1. Run the Login endpoint.
2. Copy the JWT.
3. Click **Authorize**.
4. Enter the token.
5. Test the protected endpoints.

## Unit Tests

The tests focus on the Business layer and use mocked repositories and Unit of Work objects.

Tested services include:

- CustomerService
- ProductService
- OrderService
- OrderReportService
- AuthService

Run all tests:

```bash
dotnet test
```

Run with detailed output:

```bash
dotnet test --logger "console;verbosity=detailed"
```

The tests cover both successful and failed operations, including:

- Creating, retrieving, updating, and deleting resources
- Missing resources
- Duplicate emails
- Invalid login details
- Insufficient product stock
- Transaction commits and rollbacks
- JWT generation

## Postman Collection

The Postman collection should be stored in:

```text
Postman/CustomerManagementApi.postman_collection.json
```

Recommended environment variables:

| Variable | Description |
|---|---|
| `baseUrl` | Local API URL |
| `adminToken` | Admin JWT |
| `userToken` | Regular-user JWT |
| `customerId` | Created customer ID |
| `productId` | Created product ID |
| `orderId` | Created order ID |

Recommended execution order:

```text
1. Login as Admin
2. Create Customer
3. Create Product
4. Create Order
5. Retrieve and update resources
6. Run reports
7. Run validation tests
8. Run security tests
9. Run delete requests last
```

Do not commit exported environments containing real tokens or passwords.

## Common HTTP Responses

| Status | Meaning |
|---|---|
| `200 OK` | Request completed successfully |
| `201 Created` | Resource created |
| `204 No Content` | Update or delete succeeded |
| `400 Bad Request` | Validation failed |
| `401 Unauthorized` | Missing or invalid token |
| `403 Forbidden` | User lacks permission |
| `404 Not Found` | Resource does not exist |
| `409 Conflict` | Duplicate or conflicting data |
| `429 Too Many Requests` | Rate limit exceeded |
| `500 Internal Server Error` | Unexpected server error |

## Build Verification

Before submitting the project, run:

```bash
dotnet clean
dotnet restore
dotnet build
dotnet test
```

Confirm that:

- The solution builds successfully
- All unit tests pass
- PostgreSQL migrations are applied
- Swagger opens correctly
- Authentication and authorization work
- Customer, Product, and Order operations work
- Reports return the expected results
- Postman tests pass
- No passwords, tokens, or secret keys are committed
- Temporary and unused files are removed

## Git Workflow

Check your changes:

```bash
git status
```

Stage and commit:

```bash
git add .
git commit -m "test: add unit tests and API delivery documentation"
```

Push the branch:

```bash
git push --set-upstream origin feature/week-4-testing-delivery
```

Then create a pull request into `main`.

## Author

Developed as part of a backend software development internship project.