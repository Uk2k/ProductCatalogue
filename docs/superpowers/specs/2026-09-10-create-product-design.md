# Create Product Design

## Purpose

Add the first complete Product Catalogue vertical slice: creating and reading Products through an HTTP API, validating create requests, and persisting/querying through MediatR and EF Core.

## Scope

This slice adds `POST /products`, `GET /products`, and `GET /products/{id}` with request and response contracts, MediatR commands/queries and handlers, FluentValidation for create requests, and the corresponding unit and HTTP integration tests.

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

Success returns `201 Created`, a `Location` header for `GET /products/{id}`, and the created Product representation. `GET /products` returns `200 OK` with an array of Product representations, including an empty array when no Products exist. `GET /products/{id}` returns `200 OK` with one Product representation or `404 Not Found` when the GUID does not exist. The list is returned without pagination or filtering in this slice.

## Application Flow

```text
POST /products
  -> request binding
  -> CreateProductCommand
  -> MediatR validation behavior
  -> CreateProductHandler
  -> AppDbContext.SaveChangesAsync()
  -> 201 Created

GET /products
  -> GetProductsQuery
  -> GetProductsHandler
  -> AppDbContext.Products
  -> 200 OK

GET /products/{id}
  -> GetProductByIdQuery
  -> GetProductByIdHandler
  -> AppDbContext.Products
  -> 200 OK or 404 Not Found
```

Validation is implemented with FluentValidation and invoked by a MediatR pipeline behavior. This keeps validation separate from transport concerns and makes it independently unit-testable. The handler remains responsible for creating the entity and saving it; it does not duplicate validation rules.

## Validation

The Create Product validator requires a non-blank `Name` of no more than 200 characters, a `Price` greater than zero, and `Stock` greater than or equal to zero. Invalid requests return `400 Bad Request` using `ProblemDetails` with field-level errors. The later global exception middleware will own unexpected database error responses.

## Testing

Unit tests exercise the validator directly for blank name, maximum length, invalid price, invalid stock, and a valid request.

HTTP integration tests use the existing Testcontainers SQL Server fixture and application host. They verify that a valid request returns `201`, contains a server-generated GUID and `Location` header, persists to SQL Server, and can be retrieved using `GET /products/{id}`. They verify `GET /products` returns persisted Products and an empty collection when appropriate, and that an unknown GUID returns `404`. They also verify invalid requests return `400` ProblemDetails and do not create rows. Query handlers are unit-tested for found, missing, and list behavior where practical without duplicating the HTTP tests.

## Explicit Exclusions

- PUT and DELETE endpoints.
- Repository abstraction beyond the existing EF Core context.
- Global exception middleware and OpenTelemetry changes.
- Key Vault or production deployment configuration.

## Acceptance Criteria

- `POST /products` accepts the defined request contract.
- A valid request creates a Product with a server-generated GUID and returns `201 Created`.
- The response includes a future-resource `Location` header and the created representation.
- `GET /products` returns all persisted Product representations with `200 OK`.
- `GET /products/{id}` returns the matching Product with `200 OK` or `404 Not Found` when absent.
- FluentValidation rules are covered by unit tests.
- HTTP integration tests prove create, read, persistence, not-found, and validation failure behavior against Testcontainers SQL Server.
- Existing persistence integration tests continue to pass.
