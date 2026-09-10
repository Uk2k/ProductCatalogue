using System.Diagnostics;

namespace ProductCatalogue.Api.Infrastructure.Http;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("HTTP request completed {HttpMethod} {RequestPath} with status {StatusCode} in {ElapsedMilliseconds}ms. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, context.TraceIdentifier);
        }
    }
}
