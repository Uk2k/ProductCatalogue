# Product Repository Design

## Goal

Introduce a product repository abstraction so product handlers depend on application-facing persistence operations rather than directly on EF Core.

## Design

`IProductRepository` exposes product lookup, ordered listing, add, remove, and save operations. `ProductRepository` is the EF Core implementation and is registered as a scoped service. Handlers retain ownership of mapping commands/queries to product responses; database-specific query details remain in the repository.

## Testing

Existing handler tests use the repository implementation over an isolated in-memory context, while repository tests verify lookup and ordering. Existing Testcontainers-backed HTTP and persistence tests verify the concrete SQL Server registration and behaviour.
