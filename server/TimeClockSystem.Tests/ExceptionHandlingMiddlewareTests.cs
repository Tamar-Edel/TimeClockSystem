using Microsoft.AspNetCore.Http;
using TimeClockSystem.API.Middleware;

namespace TimeClockSystem.Tests;

public class ExceptionHandlingMiddlewareTests
{
    // Creates a fresh HttpContext with a writable MemoryStream body for each test.
    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    // Reads the response body after the middleware has run.
    private static async Task<string> ReadResponseBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    // When no exception is thrown, the middleware passes through and does not alter the response.
    [Fact]
    public async Task InvokeAsync_WhenNoException_PassesThrough()
    {
        var middleware = new ExceptionHandlingMiddleware(ctx =>
        {
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        });
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal(200, context.Response.StatusCode);
    }

    // A business rule violation (InvalidOperationException) returns 400 with the exception message.
    [Fact]
    public async Task InvokeAsync_WhenInvalidOperationException_Returns400WithMessage()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new InvalidOperationException("A shift is already open."));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal(400, context.Response.StatusCode);
        var body = await ReadResponseBodyAsync(context);
        Assert.Contains("A shift is already open.", body);
    }

    // A missing resource (KeyNotFoundException) returns 404.
    [Fact]
    public async Task InvokeAsync_WhenKeyNotFoundException_Returns404()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new KeyNotFoundException("Resource not found."));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal(404, context.Response.StatusCode);
        var body = await ReadResponseBodyAsync(context);
        Assert.Contains("Resource not found.", body);
    }

    // An access violation (UnauthorizedAccessException) returns 401 with a safe generic message.
    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedAccessException_Returns401()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new UnauthorizedAccessException("Internal detail that should not leak."));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal(401, context.Response.StatusCode);
        var body = await ReadResponseBodyAsync(context);
        // The internal detail must NOT be exposed — only the safe generic message.
        Assert.Contains("Unauthorized.", body);
        Assert.DoesNotContain("Internal detail", body);
    }

    // Any other unexpected exception returns 500 without exposing internal details.
    [Fact]
    public async Task InvokeAsync_WhenUnexpectedException_Returns500WithSafeMessage()
    {
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new Exception("Internal stack trace detail."));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
        var body = await ReadResponseBodyAsync(context);
        Assert.Contains("unexpected error", body);
        Assert.DoesNotContain("Internal stack trace detail.", body);
    }
}
