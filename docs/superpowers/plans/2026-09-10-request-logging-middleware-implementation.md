# Request Logging Middleware Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add safe structured HTTP request completion logging and prove it with unit and integration tests.

**Architecture:** Create a focused middleware class under `Infrastructure/Http`, registered before endpoint mappings. Use the existing `ILogger<T>` abstraction and `Stopwatch`; no custom logging sink or request-body buffering.

**Tech Stack:** .NET 8 ASP.NET Core middleware, `ILogger`, xUnit, WebApplicationFactory, Testcontainers SQL Server.

**Spec:** `docs/superpowers/specs/2026-09-10-request-logging-middleware-design.md`

## Global Constraints

- Never log request bodies, headers, secrets, or connection strings.
- Log completion in `finally` so thrown requests are observable.
- Preserve all existing endpoint behavior and tests.

---

### Task 1: Add failing middleware unit tests

**Files:**
- Create: `tests/ProductCatalogue.UnitTests/Infrastructure/Http/RequestLoggingMiddlewareTests.cs`

- [ ] **Step 1:** Add tests for successful completion fields and exception-path completion logging using a capture `ILoggerProvider`.
- [ ] **Step 2:** Run `dotnet test tests/ProductCatalogue.UnitTests/ProductCatalogue.UnitTests.csproj --configuration Release` and confirm failure because the middleware does not exist.

### Task 2: Implement and register middleware

**Files:**
- Create: `src/ProductCatalogue.Api/Infrastructure/Http/RequestLoggingMiddleware.cs`
- Modify: `src/ProductCatalogue.Api/Program.cs`

- [ ] **Step 1:** Implement `InvokeAsync(HttpContext)` with `Stopwatch.StartNew()`, `await _next(context)` in `try`, and one structured `LogInformation` completion event in `finally`.
- [ ] **Step 2:** Register `app.UseMiddleware<RequestLoggingMiddleware>()` before health checks and Product endpoint mappings.
- [ ] **Step 3:** Run the focused unit tests and confirm they pass.

### Task 3: Add HTTP integration coverage and verify

**Files:**
- Modify: `tests/ProductCatalogue.IntegrationTests/Features/Products/ProductHttpTests.cs`

- [ ] **Step 1:** Add an HTTP request assertion proving the middleware does not change the endpoint response.
- [ ] **Step 2:** Run with Rancher Docker on PATH: `$dockerBin = 'C:\Users\alway\AppData\Local\Programs\Rancher Desktop\resources\resources\win32\bin'; $env:Path = \"$dockerBin;$env:Path\"; dotnet test ProductCatalogue.sln --configuration Release --logger \"console;verbosity=minimal\"`.
- [ ] **Step 3:** Run `git diff --check`, commit with `git commit -m \"feat: add request logging middleware\"`, and confirm temporary PNGs remain untracked and untouched.
