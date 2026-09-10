# Product Catalogue API

An ASP.NET Core .NET 8 minimal API built for the LRPQ senior software engineer development challenge.

The API provides CRUD operations for products and demonstrates vertical-slice architecture, MediatR, EF Core, SQL Server, validation, integration testing, structured request logging, exception handling, and feature-toggled OpenTelemetry.

## Technology

- .NET 8 and ASP.NET Core minimal APIs
- Entity Framework Core with SQL Server
- MediatR for request/handler dispatch
- FluentValidation for command validation
- xUnit for unit and integration tests
- Testcontainers for disposable SQL Server integration-test databases
- Swagger/OpenAPI for interactive API exploration
- OpenTelemetry for optional traces, metrics, and logs

## Prerequisites

- .NET 8 SDK
- Docker-compatible runtime, such as Rancher Desktop in dockerd mode
- Git

Verify the tools:

```powershell
dotnet --version
docker version
```

## Repository structure

```text
src/ProductCatalogue.Api
├── Data/                    EF Core context, configuration, and migrations
├── Features/Products/       Vertical slices grouped by product use case
│   ├── CreateProduct/
│   ├── GetProduct/
│   ├── GetProducts/
│   ├── UpdateProduct/
│   └── DeleteProduct/
├── Infrastructure/Http/     Request logging and exception middleware
├── Infrastructure/Observability/
│                            OpenTelemetry configuration and feature toggle
├── Program.cs                Application composition root
└── appsettings.json          Safe shared defaults

tests/ProductCatalogue.UnitTests
    Handler, validator, middleware, and registration tests

tests/ProductCatalogue.IntegrationTests
    Testcontainers SQL Server fixture and HTTP API tests
```

Each product use case owns its endpoint, request/command or query, handler, and validator where required. This keeps feature behaviour together and limits coupling between slices.

## Local setup

### 1. Configure development settings

Create `src/ProductCatalogue.Api/appsettings.Development.json` locally. Do not commit real credentials:

```json
{
  "ConnectionStrings": {
    "ProductCatalogue": "Server=127.0.0.1,14333;Database=ProductCatalogue;User Id=sa;Password=<local-password>;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Observability": {
    "OpenTelemetryEnabled": true
  }
}
```

The repository ignores this file because it contains local configuration. User Secrets, environment variables, or a Key Vault provider can supply the same keys in other environments.

### 2. Apply the database migration

```powershell
dotnet ef database update `
  --project src/ProductCatalogue.Api `
  --startup-project src/ProductCatalogue.Api
```

If `dotnet ef` is not installed:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.*
```

### 3. Run the API

```powershell
dotnet run --project src/ProductCatalogue.Api
```

The Development launch profile provides:

- Swagger: https://localhost:7065/swagger
- HTTP API: http://localhost:5207

## API endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/products` | Create a product |
| GET | `/products` | List products ordered by name |
| GET | `/products/{id}` | Get a product by GUID |
| PUT | `/products/{id}` | Replace a product |
| DELETE | `/products/{id}` | Delete a product |
| GET | `/health` | Health check |

Product identifiers are GUIDs. Updates use `PUT` because the request represents the complete product resource.

## Testing

```powershell
dotnet test ProductCatalogue.sln --configuration Release
```

Unit tests only:

```powershell
dotnet test tests/ProductCatalogue.UnitTests/ProductCatalogue.UnitTests.csproj --configuration Release
```

Integration tests only:

```powershell
dotnet test tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj --configuration Release
```

Integration tests start disposable SQL Server containers with Testcontainers, create isolated databases, apply the real EF Core migrations, and exercise persistence plus HTTP behaviour. They do not use the local development database.

## Error responses

The API uses RFC 7807 `ProblemDetails` responses:

- `400 Bad Request` for invalid product requests and malformed JSON
- `404 Not Found` when a product identifier does not exist
- `500 Internal Server Error` for unexpected failures

Unexpected error responses use generic details outside Development so stack traces and database errors are not exposed to clients.

## Logging and OpenTelemetry

`RequestLoggingMiddleware` writes structured request completion logs containing the method, path, status code, elapsed time, and trace ID.

OpenTelemetry is enabled in `appsettings.json`. When enabled, the API instruments ASP.NET Core requests, outgoing HTTP calls, EF Core/SQL operations, and application logs, exporting them to the console for local development.

Production exporter endpoints and credentials should be supplied through environment variables, User Secrets, or Key Vault rather than committed to Git.

## Configuration principles

- Shared safe defaults: `appsettings.json`
- Local development overrides: `appsettings.Development.json`
- Developer secrets: .NET User Secrets
- Deployment configuration: environment variables or a managed secret provider

The connection-string key is `ConnectionStrings:ProductCatalogue`; its environment-variable equivalent is `ConnectionStrings__ProductCatalogue`.

## Design decisions

- Vertical slices keep each use case cohesive and independently testable.
- MediatR separates HTTP binding from request handling.
- EF Core migrations provide a repeatable database schema.
- GUID IDs avoid exposing sequence information and support distributed creation.
- PUT is used for complete replacement; PATCH would require partial-update semantics and additional concurrency rules.
- Testcontainers provides real SQL Server integration tests without machine-specific dependencies.
- Middleware handles cross-cutting concerns while handlers stay focused on product behaviour.
