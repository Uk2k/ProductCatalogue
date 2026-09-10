# Exception Handling Middleware Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add globally consistent ProblemDetails responses for validation, not-found, and unexpected API failures.

**Architecture:** Add a single `ExceptionHandlingMiddleware` in the existing HTTP infrastructure folder. It maps known exceptions to typed ProblemDetails responses, logs unexpected failures, and is registered before request logging so the existing middleware observes the final response status.

**Tech Stack:** ASP.NET Core .NET 8, `ProblemDetails`, FluentValidation, xUnit, existing WebApplicationFactory and Testcontainers SQL Server fixture.

**Spec:** `docs/superpowers/specs/2026-09-10-exception-handling-design.md`

## Global Constraints

- Keep the API as a .NET 8 minimal API.
- Preserve vertical-slice feature boundaries and existing endpoint contracts.
- Do not expose stack traces or database details in non-Development responses.
- Use the existing integration-test SQL Server fixture; do not add an in-memory database.

---

### Task 1: Add failing middleware unit tests

**Files:**
- Create: `tests/ProductCatalogue.UnitTests/Infrastructure/Http/ExceptionHandlingMiddlewareTests.cs`

**Interfaces:**
- Consumes: `ProductCatalogue.Api.Infrastructure.Http.ExceptionHandlingMiddleware`.
- Produces: executable expectations for validation, not-found, and unexpected exceptions.

- [ ] **Step 1: Write tests for exception mappings**

Cover `ValidationException` as 400 with field errors, `KeyNotFoundException` as 404, and an ordinary exception as 500 with a generic detail.

- [ ] **Step 2: Run the focused tests**

Run: `dotnet test tests/ProductCatalogue.UnitTests/ProductCatalogue.UnitTests.csproj --filter FullyQualifiedName~ExceptionHandlingMiddlewareTests`

Expected: FAIL because the middleware does not exist.

### Task 2: Implement and register middleware

**Files:**
- Create: `src/ProductCatalogue.Api/Infrastructure/Http/ExceptionHandlingMiddleware.cs`
- Modify: `src/ProductCatalogue.Api/Program.cs`

**Interfaces:**
- Consumes: `RequestDelegate`, `ILogger<ExceptionHandlingMiddleware>`, `IHostEnvironment`.
- Produces: middleware that writes `ProblemDetails` or `ValidationProblemDetails` JSON.

- [ ] **Step 1: Implement exception mapping**

Use `context.Response.StatusCode`, `ContentType = "application/problem+json"`, `ProblemDetailsService`/JSON serialization compatible with ASP.NET Core, and `context.TraceIdentifier` as the instance value. Log unexpected exceptions at Error level.

- [ ] **Step 2: Register before request logging**

Add `app.UseMiddleware<ExceptionHandlingMiddleware>();` before `app.UseMiddleware<RequestLoggingMiddleware>();`.

- [ ] **Step 3: Run focused unit tests**

Run: `dotnet test tests/ProductCatalogue.UnitTests/ProductCatalogue.UnitTests.csproj --filter FullyQualifiedName~ExceptionHandlingMiddlewareTests`

Expected: PASS.

### Task 3: Add HTTP integration coverage

**Files:**
- Modify: `tests/ProductCatalogue.IntegrationTests/Features/Products/ProductHttpTests.cs` or create a focused exception-response test file beside it.

**Interfaces:**
- Consumes: existing `ProductCatalogueWebApplicationFactory` and product endpoints.
- Produces: HTTP assertions for 404 and 400 ProblemDetails payloads.

- [ ] **Step 1: Add not-found tests**

Assert GET, PUT, and DELETE for a random GUID return 404 and `application/problem+json`.

- [ ] **Step 2: Add validation response test**

POST a request with an empty name and non-positive price; assert 400 and the expected field keys.

- [ ] **Step 3: Run integration tests**

Run: `dotnet test tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj --configuration Release --verbosity normal`

Expected: PASS with Rancher Docker available.

### Task 4: Verify, document, and commit

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Document error response behaviour and local testing command**

Describe the three status mappings and show `dotnet test ProductCatalogue.sln --configuration Release`.

- [ ] **Step 2: Run complete verification**

Run: `dotnet test ProductCatalogue.sln --configuration Release --verbosity normal`

Expected: all unit and integration tests pass with zero failures.

- [ ] **Step 3: Review the diff and commit**

Run: `git diff --check` and `git status --short`, then commit with `git add src tests README.md docs && git commit -m "feat: add exception handling middleware"`.
