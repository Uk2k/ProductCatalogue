using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalogue.Api.Infrastructure.Http;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (exception is BadHttpRequestException)
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";
                await JsonSerializer.SerializeAsync(context.Response.Body, new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = "The request could not be understood.",
                    Instance = context.TraceIdentifier
                });
                return;
            }

            logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                Instance = context.TraceIdentifier
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, problem);
        }
    }
}
