using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductCatalogue.Api.Infrastructure.Http;

namespace ProductCatalogue.UnitTests.Infrastructure.Http;

public sealed class RequestLoggingMiddlewareTests
{
    [Fact]
    public async Task Logs_request_details_after_successful_request()
    {
        var sink = new TestLogSink();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(sink));
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/products";
        context.TraceIdentifier = "trace-123";
        context.Response.StatusCode = StatusCodes.Status200OK;

        await new RequestLoggingMiddleware(_ => Task.CompletedTask, loggerFactory.CreateLogger<RequestLoggingMiddleware>()).InvokeAsync(context);

        var log = Assert.Single(sink.Messages);
        Assert.Contains("GET", log);
        Assert.Contains("/products", log);
        Assert.Contains("200", log);
        Assert.Contains("trace-123", log);
    }

    [Fact]
    public async Task Logs_completion_before_rethrowing_exception()
    {
        var sink = new TestLogSink();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(sink));
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/products";

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new RequestLoggingMiddleware(_ => throw new InvalidOperationException("boom"), loggerFactory.CreateLogger<RequestLoggingMiddleware>()).InvokeAsync(context));

        Assert.Single(sink.Messages);
    }

    private sealed class TestLogSink : ILoggerProvider
    {
        public List<string> Messages { get; } = [];
        public ILogger CreateLogger(string categoryName) => new TestLogger(Messages);
        public void Dispose() { }

    }

    private sealed class TestLogger(List<string> messages) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => messages.Add(formatter(state, exception));
        private sealed class NullScope : IDisposable { public static readonly NullScope Instance = new(); public void Dispose() { } }
    }
}
