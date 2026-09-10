# Feature-Toggled OpenTelemetry Design

## Goal

Add optional OpenTelemetry instrumentation for the API while keeping the default local and test experience unchanged.

## Behaviour

- `Observability:OpenTelemetryEnabled` controls registration.
- The default is `false` when the setting is absent.
- When enabled, ASP.NET Core requests, outgoing `HttpClient` calls, and EF Core/SQL operations are instrumented.
- Development uses a console exporter so telemetry can be seen without paid infrastructure.
- Exporter configuration remains external and can be supplied later through environment variables, User Secrets, or Key Vault.
- When disabled, no OpenTelemetry providers or exporters are registered.

## Design

Create a focused `AddProductCatalogueObservability` service-registration extension. It reads a strongly typed options value and conditionally adds OpenTelemetry tracing, metrics, and logging integration. Program.cs remains a composition root and does not contain exporter details.

## Testing

- Unit tests verify the configuration default and enabled/disabled registration decisions.
- Existing API unit and integration tests continue to prove that telemetry is non-invasive.
- A smoke test starts the API with telemetry enabled and verifies normal HTTP behaviour.

## Non-goals

- No collector, Jaeger, Aspire Dashboard, Azure Monitor, or paid service is required.
- No production exporter endpoint or credential is committed.
- No custom business spans are added in this slice; those can be added around important use cases later.
