# ProductCatalogue
Dev challenge for LRPQ senios software engineer role.

## Error responses

The API returns RFC 7807 ProblemDetails responses for client-visible errors:

- `400 Bad Request` for invalid product requests, including field-level validation errors.
- `404 Not Found` when a product identifier does not exist.
- `500 Internal Server Error` for unexpected failures, with a generic detail outside Development.

Run the complete test suite with:

```powershell
dotnet test ProductCatalogue.sln --configuration Release
```

## OpenTelemetry

OpenTelemetry is disabled by default. Enable it for local development with:

```powershell
$env:Observability__OpenTelemetryEnabled = "true"
dotnet run --project src/ProductCatalogue.Api
```

When enabled, ASP.NET Core requests, outgoing HTTP calls, EF Core/SQL operations, and application logs are exported to the console. A deployed OTLP exporter endpoint and credentials should be supplied through environment variables, User Secrets, or Key Vault rather than committed to the repository.
