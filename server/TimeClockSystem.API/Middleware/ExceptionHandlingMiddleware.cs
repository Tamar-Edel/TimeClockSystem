using System.Text.Json;

namespace TimeClockSystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { message = ex.Message });
            await context.Response.WriteAsync(body);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { message = "An unexpected error occurred." });
            await context.Response.WriteAsync(body);
        }
    }
}
