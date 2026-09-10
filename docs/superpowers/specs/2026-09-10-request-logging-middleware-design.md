# Request Logging Middleware Design

## Purpose

Add the request logging middleware required by the coding challenge as a cross-cutting slice before the remaining Product endpoints.

## Scope

The middleware logs one structured completion event for every HTTP request, including method, path, status code, elapsed time, and the ASP.NET Core request trace identifier. It must log completion in a `finally` block so requests that throw are also observable, without logging request bodies, headers, secrets, or connection strings.

The middleware is registered early in the pipeline so it surrounds health checks and Product endpoints. The existing framework logging provider remains responsible for output and filtering.

## Testing

Unit tests invoke the middleware with a test `RequestDelegate` and an in-memory `ILogger` provider, proving method/path/status/duration/trace ID fields are emitted and that completion logging occurs when the delegate throws. An HTTP integration test proves a real request passes through the middleware and returns the endpoint response unchanged.

## Explicit Exclusions

- Request/response body logging.
- Authentication or user identity enrichment.
- Distributed tracing and OpenTelemetry changes.
- Global exception handling; that is a separate cross-cutting slice.
