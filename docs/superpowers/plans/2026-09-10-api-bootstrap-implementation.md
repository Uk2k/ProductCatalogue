# API Bootstrap Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create a runnable .NET 8 minimal API host with development Swagger/OpenAPI and a health endpoint.

**Architecture:** A single `ProductCatalogue.Api` web project is the initial composition root. It exposes only operational HTTP concerns: health checking and development-only API exploration; data access, product features, tests, and cross-cutting application behaviour remain outside this slice.

**Tech Stack:** .NET 8, ASP.NET Core minimal APIs, Swashbuckle.AspNetCore 6.6.2.

**Spec:** `docs/superpowers/specs/2026-09-10-api-bootstrap-design.md`

## Global Constraints

- Require a .NET 8 SDK before execution; `dotnet --list-sdks` must show an `8.0.*` SDK.
- Target `net8.0` and use nullable reference types.
- Keep this slice to one minimal API project; do not add EF Core, SQL Server, MediatR, test projects, request logging, exception handling, OpenTelemetry, Key Vault, or product routes.
- Enable Swagger and Swagger UI only when `IHostEnvironment.IsDevelopment()` is true.
- Expose `GET /health` using ASP.NET Core health checks and preserve a public `partial Program` class for the later test host.

---

## File Structure

| Path | Responsibility |
|---|---|
| `ProductCatalogue.sln` | Solution entry point for the API now and later slices. |
| `src/ProductCatalogue.Api/ProductCatalogue.Api.csproj` | .NET 8 web-project settings and Swagger dependency. |
| `src/ProductCatalogue.Api/Program.cs` | Host composition, operational service registration, and route mapping. |
| `.gitignore` | Preserves existing Visual Studio exclusions and adds project-specific local secrets and database files. |

### Task 1: Create the solution and web-project skeleton

**Files:**
- Create: `ProductCatalogue.sln`
- Create: `src/ProductCatalogue.Api/ProductCatalogue.Api.csproj`
- Create: `src/ProductCatalogue.Api/Program.cs`
- Modify: `.gitignore`

**Interfaces:**
- Consumes: .NET 8 SDK command-line templates.
- Produces: `ProductCatalogue.Api`, an executable ASP.NET Core web project referenced by `ProductCatalogue.sln`.

- [ ] **Step 1: Verify the .NET 8 SDK precondition**

Run:

```powershell
dotnet --list-sdks
```

Expected: at least one installed SDK version begins with `8.0.`. If no SDK is listed, stop implementation and install the .NET 8 SDK; do not substitute a preview or newer target framework.

- [ ] **Step 2: Scaffold the solution and empty web project**

Run:

```powershell
dotnet new sln --name ProductCatalogue
dotnet new web --name ProductCatalogue.Api --output src/ProductCatalogue.Api --framework net8.0
dotnet sln ProductCatalogue.sln add src/ProductCatalogue.Api/ProductCatalogue.Api.csproj
```

Expected: `ProductCatalogue.sln` references exactly `src/ProductCatalogue.Api/ProductCatalogue.Api.csproj`.

- [ ] **Step 3: Add the Swagger dependency**

Run:

```powershell
dotnet add src/ProductCatalogue.Api/ProductCatalogue.Api.csproj package Swashbuckle.AspNetCore --version 6.6.2
```

Expected: the project file contains a `PackageReference` for `Swashbuckle.AspNetCore` version `6.6.2`.

- [ ] **Step 4: Set the project’s compile-time conventions**

Replace `src/ProductCatalogue.Api/ProductCatalogue.Api.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
  </ItemGroup>
</Project>
```

- [ ] **Step 5: Preserve and extend source-control exclusions**

Keep the existing Visual Studio `.gitignore` content. Append only these entries when they are not already present:

```gitignore
*.db
*.db-shm
*.db-wal
appsettings.Development.json
secrets.json
coverage/
```

- [ ] **Step 6: Build the skeleton**

Run:

```powershell
dotnet build ProductCatalogue.sln --configuration Release
```

Expected: build succeeds with zero errors.

### Task 2: Compose the operational API host

**Files:**
- Modify: `src/ProductCatalogue.Api/Program.cs`
- Test: Manual HTTP verification; automated test projects are explicitly deferred by the spec.

**Interfaces:**
- Consumes: ASP.NET Core `WebApplication`, `IHostEnvironment`, Swashbuckle registration methods, and ASP.NET Core health checks.
- Produces: `GET /health`, Swagger JSON at `/swagger/v1/swagger.json`, Swagger UI at `/swagger` in Development, and `public partial class Program` for the later `WebApplicationFactory<Program>` fixture.

- [ ] **Step 1: Implement the minimal host composition**

Replace `src/ProductCatalogue.Api/Program.cs` with:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
```

- [ ] **Step 2: Build the composed host**

Run:

```powershell
dotnet build ProductCatalogue.sln --configuration Release
```

Expected: build succeeds with zero errors and no product, persistence, or test-project dependency is present.

- [ ] **Step 3: Run the API in Development**

Run in one terminal:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/ProductCatalogue.Api --urls http://127.0.0.1:5099
```

Expected: the host reports that it is listening on `http://127.0.0.1:5099`.

- [ ] **Step 4: Verify the health endpoint**

Run in a second terminal while the host is running:

```powershell
(Invoke-WebRequest http://127.0.0.1:5099/health).StatusCode
```

Expected: `200`.

- [ ] **Step 5: Verify the OpenAPI document and Swagger UI**

Run:

```powershell
(Invoke-WebRequest http://127.0.0.1:5099/swagger/v1/swagger.json).StatusCode
(Invoke-WebRequest http://127.0.0.1:5099/swagger/index.html).StatusCode
```

Expected: both commands return `200` in the Development environment.

- [ ] **Step 6: Stop the development host**

In the terminal running `dotnet run`, press `Ctrl+C`.

Expected: the application exits cleanly without leaving a background process.

- [ ] **Step 7: Commit the API bootstrap**

Run:

```powershell
git add .gitignore ProductCatalogue.sln src/ProductCatalogue.Api
git commit -m "feat: bootstrap minimal API host"
```

Expected: the commit contains only the solution, API project, and `.gitignore`; the approved spec and plan remain in their earlier documentation commits.

## Plan Self-Review

### Spec coverage

- Solution and `net8.0` web project: Task 1.
- Development-only Swagger/OpenAPI: Task 2, steps 1, 3, and 5.
- `GET /health`: Task 2, steps 1 and 4.
- Public `partial Program`: Task 2, step 1.
- `.gitignore`: Task 1, step 5.
- Deferred scope: enforced by the global constraints and Task 2 build check.

### Consistency checks

- The project name, assembly entry point, endpoint path, port, and Swagger paths are consistent across all tasks.
- The plan intentionally uses manual HTTP verification because the approved specification defers creation of all test projects to Slice 3.

