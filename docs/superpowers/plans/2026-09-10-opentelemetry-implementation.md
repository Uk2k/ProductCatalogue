# Feature-Toggled OpenTelemetry Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add optional OpenTelemetry instrumentation controlled by configuration.

**Architecture:** Put configuration and conditional provider registration in `Infrastructure/Observability`. Program.cs calls one extension method. The default is disabled; enabled Development exports telemetry to the console and all exporter choices remain externally configurable.

**Tech Stack:** .NET 8, OpenTelemetry.Extensions.Hosting, OpenTelemetry.Exporter.Console, OpenTelemetry.Instrumentation.AspNetCore, OpenTelemetry.Instrumentation.Http, OpenTelemetry.Instrumentation.EntityFrameworkCore, xUnit.

**Spec:** `docs/superpowers/specs/2026-09-10-opentelemetry-design.md`

## Global Constraints

- Keep OpenTelemetry disabled by default.
- Do not commit endpoints, credentials, or connection strings.
- Preserve the minimal API and vertical-slice structure.
- Keep telemetry registration outside product feature folders.

---

### Task 1: Add options and registration tests

**Files:**
- Create: `src/ProductCatalogue.Api/Infrastructure/Observability/ObservabilityOptions.cs`
- Create: `src/ProductCatalogue.Api/Infrastructure/Observability/ObservabilityServiceCollectionExtensions.cs`
- Create: `tests/ProductCatalogue.UnitTests/Infrastructure/Observability/ObservabilityRegistrationTests.cs`

**Interfaces:**
- Consumes: `IConfiguration` key `Observability:OpenTelemetryEnabled`.
- Produces: `IServiceCollection AddProductCatalogueObservability(IConfiguration)`.

- [ ] **Step 1: Write failing tests**

Assert absent configuration produces disabled options and a true configuration value produces enabled options.

- [ ] **Step 2: Run focused tests and verify failure**

Run: `dotnet test tests/ProductCatalogue.UnitTests/ProductCatalogue.UnitTests.csproj --filter FullyQualifiedName~ObservabilityRegistrationTests`

Expected: FAIL because the options and extension do not exist.

### Task 2: Add OpenTelemetry dependencies and conditional registration

**Files:**
- Modify: `src/ProductCatalogue.Api/ProductCatalogue.Api.csproj`
- Modify: `src/ProductCatalogue.Api/Infrastructure/Observability/ObservabilityServiceCollectionExtensions.cs`
- Modify: `src/ProductCatalogue.Api/Program.cs`

**Interfaces:**
- Consumes: `ObservabilityOptions` and `IConfiguration`.
- Produces: conditional tracing/metrics/logging providers with ASP.NET Core, HttpClient, EF Core, and console instrumentation.

- [ ] **Step 1: Add OpenTelemetry packages**

Add the hosting, console exporter, ASP.NET Core, HttpClient, and Entity Framework Core instrumentation packages at one compatible OpenTelemetry version.

- [ ] **Step 2: Register providers only when enabled**

Bind `ObservabilityOptions`, return without adding providers when disabled, and configure ASP.NET Core, HttpClient, and EF Core instrumentation plus console exporters when enabled.

- [ ] **Step 3: Register the extension in Program.cs**

Call `builder.Services.AddProductCatalogueObservability(builder.Configuration);` before `builder.Build()`.

- [ ] **Step 4: Run focused tests**

Run: `dotnet test tests/ProductCatalogue.UnitTests/ProductCatalogue.UnitTests.csproj --filter FullyQualifiedName~ObservabilityRegistrationTests`

Expected: PASS.

### Task 3: Add configuration and documentation

**Files:**
- Modify: `src/ProductCatalogue.Api/appsettings.json`
- Modify: `README.md`

- [ ] **Step 1: Add the disabled default**

Add `Observability:OpenTelemetryEnabled` as `false` without adding exporter URLs or credentials.

- [ ] **Step 2: Document configuration**

Document `$env:Observability__OpenTelemetryEnabled = "true"`, console output for local development, and external configuration of future OTLP exporters.

### Task 4: Verify and commit

- [ ] **Step 1: Run complete verification**

Run: `dotnet test ProductCatalogue.sln --configuration Release --verbosity normal`

Expected: all unit and integration tests pass.

- [ ] **Step 2: Review and commit**

Run `git diff --check`, inspect the diff, then commit with `git commit -m "feat: add feature-toggled OpenTelemetry"`.
