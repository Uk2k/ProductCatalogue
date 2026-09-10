# Persistence Integration Tests Design

## Purpose

Establish repeatable, production-faithful integration coverage for the Product Catalogue persistence boundary before product HTTP endpoints exist.

## Scope

This slice creates a single integration-test project that runs against a disposable SQL Server instance managed by Testcontainers. It proves that the committed EF Core migration, `AppDbContext`, and SQL Server integrity constraints operate together.

The slice does not create a unit-test project or validators. Unit tests will be added with the Create and Update Product vertical slices, where request validation and handler behaviour exist to test.

## Architecture

`tests/ProductCatalogue.IntegrationTests` references `ProductCatalogue.Api` and uses xUnit, Testcontainers for SQL Server, and EF Core SQL Server. The tests use the production `AppDbContext` and its committed migrations; they do not substitute an in-memory database or hand-maintained SQL schema.

A shared xUnit collection fixture owns one SQL Server container for the test run. For every test, the fixture creates a uniquely named database, applies migrations with `Database.MigrateAsync()`, and supplies a connection-string-backed context factory. It drops that database after the test completes. This keeps container startup cost low while preventing test state leaking between tests.

```text
xUnit test
  -> shared SQL Server Testcontainer
    -> unique test database
      -> Database.MigrateAsync()
        -> production InitialCreate migration
          -> AppDbContext assertion
```

## Initial Coverage

- Migration test: applying `InitialCreate` creates the `Products` table.
- Persistence round-trip: a valid `Product` saves and is read by its GUID.
- Database integrity: SQL Server rejects `Price <= 0` and `Stock < 0` through the configured check constraints.

These are persistence integration tests, not HTTP integration tests. Product API endpoint coverage begins with the first endpoint slice.

## Configuration and Failure Behaviour

Tests obtain their SQL Server connection exclusively from the Testcontainers fixture. They neither use User Secrets nor connect to LocalDB or a developer-owned database.

Docker/Rancher Desktop must be available when integration tests run. Container startup, migration, or cleanup failures fail the affected test with the original error; the suite does not skip or silently downgrade database coverage.

## Explicit Exclusions

- Minimal API endpoint hosting and `WebApplicationFactory`.
- MediatR handlers, request DTOs, FluentValidation, and unit tests.
- Test data builders beyond the small data required by these persistence tests.
- OpenTelemetry, middleware, and API error handling.

## Acceptance Criteria

- The solution includes a runnable integration-test project.
- Testcontainers starts SQL Server through Docker/Rancher Desktop.
- Each test uses an isolated database generated from the committed migration.
- Tests prove valid Product persistence and both SQL Server check constraints.
- The suite has no dependency on developer-local secrets or LocalDB.
