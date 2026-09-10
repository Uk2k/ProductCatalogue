using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using ProductCatalogue.Api.Infrastructure.Http;

namespace ProductCatalogue.UnitTests.Infrastructure.Http;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenUnhandledExceptionOccurs_ReturnsGenericProblemDetails()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("database details"),
            NullLogger<ExceptionHandlingMiddleware>.Instance,
            new TestHostEnvironment { EnvironmentName = Environments.Production });
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-123"
        };
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await middleware.InvokeAsync(context);

        responseBody.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetailsDto>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal("An unexpected error occurred.", problem?.Detail);
        Assert.Equal("trace-123", problem?.Instance);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "Tests";
        public string WebRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; } = string.Empty;
    }

    private sealed record ProblemDetailsDto(string? Detail, string? Instance);
}
