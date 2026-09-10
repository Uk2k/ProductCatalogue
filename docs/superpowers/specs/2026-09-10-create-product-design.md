# Create Product Design

## Purpose

Add the first complete Product Catalogue vertical slice: creating a Product through an HTTP API, validating the request, and persisting it through MediatR and EF Core.

## Scope

This slice adds `POST /products` with request and response contracts, a MediatR command and handler, FluentValidation, and the corresponding unit and HTTP integration tests.

The server generates the Product GUID. The handler persists `Name`, `Price`, and `Stock` through the existing `AppDbContext` and returns the created representation.

## Request and Response

Request body:

```json
{
  "name": "Keyboard",
  "price": 49.99,
  "stock": 10
}
```

Success returns `201 Created`, a `Location` header for the planned `GET /products/{id}` route, and the created Product representation. No GET endpoint is implemented in this slice.

## Application Flow

```text
POST /products
  -> request binding
  -> CreateProductCommand
  -> MediatR validation behavior
  -> CreateProductHandler
  -> AppDbContext.SaveChangesAsync()
  -> 201 Created
```

Validation is implemented with FluentValidation and invoked by a MediatR pipeline behavior. This keeps validation separate from transport concerns and makes it independently unit-testable. The handler remains responsible for creating the entity and saving it; it does not duplicate validation rules.

## Validation

The Create Product validator requires a non-blank `Name` of no more than 200 characters, a `Price` greater than zero, and `Stock` greater than or equal to zero. Invalid requests return `400 Bad Request` using `ProblemDetails` with field-level errors. The later global exception middleware will own unexpected database error responses.

## Testing

Unit tests exercise the validator directly for blank name, maximum length, invalid price, invalid stock, and a valid request.

HTTP integration tests use the existing Testcontainers SQL Server fixture and application host. They verify that a valid request returns `201`, contains a server-generated GUID and `Location` header, and persists to SQL Server. They also verify invalid requests return `400` ProblemDetails and do not create rows.

## Explicit Exclusions

- GET, PUT, and DELETE endpoints.
- Repository abstraction beyond the existing EF Core context.
- Global exception middleware and OpenTelemetry changes.
- Key Vault or production deployment configuration.

## Acceptance Criteria

- `POST /products` accepts the defined request contract.
- A valid request creates a Product with a server-generated GUID and returns `201 Created`.
- The response includes a future-resource `Location` header and the created representation.
- FluentValidation rules are covered by unit tests.
- HTTP integration tests prove success, persistence, and validation failure behavior against Testcontainers SQL Server.
- Existing persistence integration tests continue to pass.
