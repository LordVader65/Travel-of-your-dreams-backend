using System.Net;
using System.Text.Json;

namespace TravelDreams.MsIdentidad.Api.Middleware;

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
            await WriteErrorAsync(context, ex);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var status = ResolveStatusCode(ex);
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

    private static int ResolveStatusCode(Exception ex)
    {
        var message = ex.Message.ToLowerInvariant();
        if (message.Contains("credenciales invalidas") || message.Contains("usuario inactivo")) return StatusCodes.Status401Unauthorized;
        if (message.Contains("no tiene rol") || message.Contains("no puede desactivarse") || message.Contains("no puede cambiar")) return StatusCodes.Status403Forbidden;
        if (message.Contains("no encontrado") || message.Contains("no encontrada")) return StatusCodes.Status404NotFound;
        if (message.Contains("ya existe") || message.Contains("ya esta") || message.Contains("duplic")) return StatusCodes.Status409Conflict;
        if (ex is InvalidOperationException or ArgumentException or FormatException) return StatusCodes.Status400BadRequest;
        return StatusCodes.Status500InternalServerError;
    }
}
