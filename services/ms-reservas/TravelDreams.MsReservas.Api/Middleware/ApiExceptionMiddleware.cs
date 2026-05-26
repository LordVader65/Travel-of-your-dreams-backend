using Grpc.Core;
using System.Text.Json;

namespace TravelDreams.MsReservas.Api.Middleware;

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
        var message = GetClientMessage(ex);
        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            status,
            error = status == StatusCodes.Status500InternalServerError ? "Error interno del servidor." : message,
            details = status == StatusCodes.Status500InternalServerError ? [] : new[] { message },
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.Value
        }));
    }

    private static string GetClientMessage(Exception ex)
    {
        if (ex is RpcException rpc && !string.IsNullOrWhiteSpace(rpc.Status.Detail))
        {
            return rpc.Status.Detail;
        }

        return ex.Message;
    }

    private static int ResolveStatusCode(Exception ex)
    {
        if (ex is RpcException rpc) return rpc.StatusCode switch
        {
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.FailedPrecondition or StatusCode.Aborted or StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
            StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status502BadGateway
        };

        var message = ex.Message.ToLowerInvariant();
        if (message.Contains("no pertenece")) return StatusCodes.Status403Forbidden;
        if (message.Contains("no encontrado") || message.Contains("no encontrada")) return StatusCodes.Status404NotFound;
        if (message.Contains("ya existe") || message.Contains("solo se puede") || message.Contains("cupos") || message.Contains("expirada") || message.Contains("pendiente")) return StatusCodes.Status409Conflict;
        if (ex is InvalidOperationException or ArgumentException or FormatException) return StatusCodes.Status400BadRequest;
        return StatusCodes.Status500InternalServerError;
    }
}
