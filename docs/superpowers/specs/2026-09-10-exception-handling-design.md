# Exception Handling Middleware Design

## Goal

Provide consistent RFC 7807 `ProblemDetails` responses for expected and unexpected API failures without leaking implementation details.

## Scope

This slice adds one global middleware to the ASP.NET Core pipeline and tests its HTTP behaviour. It does not change product handlers or introduce a new exception hierarchy.

## Behaviour

- Existing endpoint `404` and validation `400` results remain endpoint-owned, but are standardized through ASP.NET Core ProblemDetails configuration.
- Any other exception becomes HTTP 500 with a generic `ProblemDetails` body.
- In Development, the unexpected-error detail may contain the exception message; non-Development responses use a generic detail.
- The middleware logs unexpected exceptions with the exception and request context.
- Responses use `application/problem+json` and preserve the request trace identifier.

## Design

`ExceptionHandlingMiddleware` wraps the remaining pipeline and writes a response only when an unexpected exception escapes. `AddProblemDetails` provides the common response contract for endpoint-generated client errors. Registration occurs before request logging so the logging middleware records the final status code.

## Testing

- Unit tests invoke the middleware with a minimal `RequestDelegate`, a real `DefaultHttpContext`, and a test logger.
- Integration tests exercise GET, PUT, and DELETE not-found responses plus validation responses through the existing Testcontainers-backed web application factory.
- A failing delegate verifies the generic 500 response and that implementation details are not exposed outside Development.

## Non-goals

- Retry policies, domain exception types, authentication/authorization errors, and telemetry exporters are separate slices.
