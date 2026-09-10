# Persistence Integration Tests Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add repeatable SQL Server persistence integration tests backed by a disposable Testcontainers database.

**Architecture:** A shared xUnit collection fixture owns one SQL Server container. Each test obtains a uniquely named database, applies the production EF migrations, and receives an `AppDbContext` configured for that database. The fixture drops the database after the test, isolating state without restarting the container for every test.

**Tech Stack:** .NET 8, xUnit, Testcontainers.MsSql, Microsoft.EntityFrameworkCore.SqlServer 8.0.31, SQL Server Linux container, Docker/Rancher Desktop.

**Spec:** `docs/superpowers/specs/2026-09-10-persistence-integration-tests-design.md`

## Global Constraints

- Use Testcontainers for SQL Server; do not use LocalDB, User Secrets, or an EF Core in-memory provider.
- Use the production `AppDbContext` and committed migrations; do not duplicate the schema in test code.
- Use a unique database per test and remove it during fixture cleanup.
- Do not add HTTP endpoint tests, validators, MediatR handlers, or a unit-test project in this slice.
- Docker/Rancher Desktop is a prerequisite; do not skip integration tests when it is unavailable.

---

## File Structure

| Path | Responsibility |
|---|---|
| `tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj` | Test project, API project reference, and test dependencies. |
| `tests/ProductCatalogue.IntegrationTests/TestFramework/SqlServerContainerFixture.cs` | Shared SQL Server container and isolated database lifecycle. |
| `tests/ProductCatalogue.IntegrationTests/TestFramework/IntegrationTestCollection.cs` | xUnit collection definition for one shared fixture. |
| `tests/ProductCatalogue.IntegrationTests/PersistenceTests.cs` | Migration, round-trip, and constraint integration tests. |
| `ProductCatalogue.sln` | Includes the integration-test project. |

### Task 1: Verify the Docker prerequisite and scaffold the test project

**Files:**
- Create: `tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj`
- Create: `tests/ProductCatalogue.IntegrationTests/Usings.cs`
- Modify: `ProductCatalogue.sln`

**Interfaces:**
- Consumes: `src/ProductCatalogue.Api/ProductCatalogue.Api.csproj`.
- Produces: a runnable xUnit project targeting `net8.0`.

- [ ] **Step 1: Verify Rancher/Docker is reachable**

Run from the repository root:

```powershell
docker info
```

Expected: Docker client and server information is returned. If the command is unavailable or the daemon is stopped, pause and ask the developer to start Rancher Desktop before continuing.

- [ ] **Step 2: Create the xUnit project**

Run:

```powershell
dotnet new xunit --name ProductCatalogue.IntegrationTests --output tests/ProductCatalogue.IntegrationTests --framework net8.0
dotnet sln ProductCatalogue.sln add tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj
dotnet add tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj reference src/ProductCatalogue.Api/ProductCatalogue.Api.csproj
```

- [ ] **Step 3: Add the integration dependencies**

Run:

```powershell
dotnet add tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj package Testcontainers.MsSql --version 4.7.0
dotnet add tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.31
```

Remove the generated `UnitTest1.cs` and add `Usings.cs` with:

```csharp
global using Microsoft.EntityFrameworkCore;
global using ProductCatalogue.Api.Data;
global using ProductCatalogue.Api.Features.Products;
global using Xunit;
```

- [ ] **Step 4: Build the scaffold**

Run:

```powershell
dotnet build ProductCatalogue.sln --configuration Release
```

Expected: the solution builds with zero errors.

- [ ] **Step 5: Commit the scaffold**

```powershell
git add ProductCatalogue.sln tests/ProductCatalogue.IntegrationTests
git commit -m "test: scaffold persistence integration tests"
```

### Task 2: Build the shared SQL Server fixture

**Files:**
- Create: `tests/ProductCatalogue.IntegrationTests/TestFramework/SqlServerContainerFixture.cs`
- Create: `tests/ProductCatalogue.IntegrationTests/TestFramework/IntegrationTestCollection.cs`

**Interfaces:**
- Consumes: `AppDbContext`, `ProductCatalogue.Api` migrations, and Testcontainers `MsSqlBuilder`.
- Produces: `SqlServerContainerFixture.CreateDatabaseAsync()` returning an isolated `DatabaseScope` with an `AppDbContext` and async cleanup.

- [ ] **Step 1: Add the collection contract**

Create:

```csharp
namespace ProductCatalogue.IntegrationTests.TestFramework;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<SqlServerContainerFixture>
{
    public const string Name = "SQL Server integration tests";
}
```

- [ ] **Step 2: Add the container fixture**

Create `SqlServerContainerFixture` implementing `IAsyncLifetime`. Configure `MsSqlBuilder` with a strong test-only password and a fixed SQL Server image tag. Start the container in `InitializeAsync()` and dispose it in `DisposeAsync()`.

The fixture must expose:

```csharp
public Task<DatabaseScope> CreateDatabaseAsync();
```

`CreateDatabaseAsync` must generate a database name such as `ProductCatalogueTest_` followed by a GUID without hyphens, build a connection string from the container’s server connection string plus that database name, create `DbContextOptions<AppDbContext>` with `UseSqlServer`, call `Database.MigrateAsync()`, and return a scope that owns the context and drops the database on disposal.

The scope cleanup must dispose its context first, open a separate master context using the container connection string, execute a parameterized `DROP DATABASE` command, and dispose the master context. Do not interpolate an uncontrolled value into SQL; validate the generated database name or use a SQL parameter for the termination query.

- [ ] **Step 3: Verify fixture compilation**

Run:

```powershell
dotnet build tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj --configuration Release
```

Expected: build succeeds with zero errors.

- [ ] **Step 4: Commit the fixture**

```powershell
git add tests/ProductCatalogue.IntegrationTests/TestFramework
git commit -m "test: add isolated SQL Server container fixture"
```

### Task 3: Add migration and persistence integration tests

**Files:**
- Create: `tests/ProductCatalogue.IntegrationTests/PersistenceTests.cs`

**Interfaces:**
- Consumes: `SqlServerContainerFixture`, `IntegrationTestCollection`, and `DatabaseScope`.
- Produces: three integration tests proving migration, Product round-trip persistence, and both database check constraints.

- [ ] **Step 1: Add the migration test**

Create a test class using `[Collection(IntegrationTestCollection.Name)]` and constructor injection of `SqlServerContainerFixture`. Add a fact that creates a scope, calls `context.Database.GetPendingMigrationsAsync()`, and asserts the result is empty. Also assert `context.Products` can be queried, proving `Products` exists after migration.

- [ ] **Step 2: Add the valid Product round-trip test**

Create a new `Product` with an explicit `Guid`, a non-empty name, a positive decimal price, and non-negative stock. Add it through `context.Products`, call `SaveChangesAsync()`, dispose that context, then create a second context for the same scope database and assert the row is returned with matching values.

- [ ] **Step 3: Add the price constraint test**

Add a Product with `Price = 0`, call `SaveChangesAsync()`, and assert a `DbUpdateException` is thrown. Inspect the exception chain for the `CK_Products_Price_Positive` constraint name so the test proves the database constraint—not only application validation—rejected the write.

- [ ] **Step 4: Add the stock constraint test**

Add a Product with `Stock = -1`, call `SaveChangesAsync()`, and assert a `DbUpdateException` whose exception chain contains `CK_Products_Stock_NonNegative`.

- [ ] **Step 5: Run the focused integration tests**

Run:

```powershell
dotnet test tests/ProductCatalogue.IntegrationTests/ProductCatalogue.IntegrationTests.csproj --configuration Release --verbosity normal
```

Expected: four tests pass. Docker/Rancher Desktop must remain running for the full command.

- [ ] **Step 6: Run the complete solution test suite**

Run:

```powershell
dotnet test ProductCatalogue.sln --configuration Release --verbosity normal
```

Expected: the four integration tests pass and no test project reports failures.

- [ ] **Step 7: Commit the tests**

```powershell
git add tests/ProductCatalogue.IntegrationTests/PersistenceTests.cs
git commit -m "test: cover product persistence schema and constraints"
```

## Plan Self-Review

- The spec’s container, migration, isolated-database, round-trip, and constraint requirements are covered by Tasks 1 through 3.
- Endpoint, validator, unit-test, and HTTP-host concerns remain explicitly excluded.
- No developer secrets or LocalDB configuration is used.
- The fixture API, collection name, database scope, and test project paths are defined before consumers use them.
