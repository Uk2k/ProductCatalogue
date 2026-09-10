# API Bootstrap Design

## Purpose

Establish a small, runnable .NET 8 minimal API host for the Product Catalogue coding challenge. This slice proves the application can build, start, and expose basic operational endpoints before persistence, product features, or cross-cutting concerns are added.

## Scope

This slice creates:

- A .NET solution named `ProductCatalogue.sln`.
- A `src/ProductCatalogue.Api` ASP.NET Core minimal API project targeting .NET 8.
- Development-only Swagger/OpenAPI support.
- `GET /health`, returning `200 OK` to indicate that the host is running.
- A `public partial class Program` marker that allows the future integration-test project to use `WebApplicationFactory<Program>`.
- A .NET-focused `.gitignore` for build output, IDE state, local secrets, and local database artefacts.

## Design

`Program.cs` will be the composition root for this initial host. It will register endpoint exploration, OpenAPI generation, Swagger UI, and health checks. At runtime it will enable Swagger only in development, map `GET /health`, and start the application.

The health route is deliberately small and does not validate a database connection. SQL Server and EF Core belong to the next persistence slice; this endpoint answers only whether the HTTP application host is available.

The project will expose `public partial class Program` after the top-level application statements. This leaves the production startup model unchanged while giving the future integration test project a stable, compile-time application entry point.

## Explicit Exclusions

The following are deferred to later slices:

- `Product` entity, EF Core, SQL Server configuration, and migrations.
- Product routes and MediatR.
- Validation, request logging, error handling, and OpenTelemetry.
- Unit and integration test projects, including Testcontainers.
- Key Vault configuration, deployment-variable templates, and the final README.

## Acceptance Criteria

- `dotnet build ProductCatalogue.sln` succeeds using the .NET 8 SDK.
- `dotnet run --project src/ProductCatalogue.Api` starts the API locally.
- `GET /health` returns `200 OK`.
- Swagger UI and the OpenAPI document are available in the Development environment.
- No database, product-specific, or test-infrastructure dependency is introduced in this slice.

## Verification

The implementation plan will run the solution build and start the API in the Development environment. It will verify the health response and Swagger endpoint with HTTP requests.
