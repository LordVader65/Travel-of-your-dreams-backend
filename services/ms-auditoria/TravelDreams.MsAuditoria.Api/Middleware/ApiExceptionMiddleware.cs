using System.Text.Json;

namespace TravelDreams.MsAuditoria.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var status = ex is InvalidOperationException or ArgumentException ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
            if (status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning(ex, "Business exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status,
                error = status == StatusCodes.Status500InternalServerError ? "Error interno del servidor." : ex.Message,
                details = status == StatusCodes.Status500InternalServerError ? [] : new[] { ex.Message },
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value
            }));
        }
    }
}
