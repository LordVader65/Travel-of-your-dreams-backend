using AtraccionesTuristicas.Backend.LA.Api.Models.Common;
using AtraccionesTuristicas.Backend.LA.Business.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AtraccionesTuristicas.Backend.LA.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (AtraccionesTuristicas.Backend.LA.Business.Exceptions.ValidationException ex)
        {
            _logger.LogWarning(ex, "Se produjo una excepción de validación.");

            var response = ApiErrorResponse.Create(
                status: StatusCodes.Status400BadRequest,
                error: "La solicitud es inválida.",
                path: context.Request.Path,
                details: ex.Errors);

            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, response);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró el recurso solicitado.");

            var response = ApiErrorResponse.Create(
                status: StatusCodes.Status404NotFound,
                error: ex.Message,
                path: context.Request.Path);

            await WriteResponseAsync(context, StatusCodes.Status404NotFound, response);
        }
        catch (UnauthorizedBusinessException ex)
        {
            _logger.LogWarning(ex, "Se produjo una excepción de autorización.");

            var response = ApiErrorResponse.Create(
                status: StatusCodes.Status401Unauthorized,
                error: ex.Message,
                path: context.Request.Path);

            await WriteResponseAsync(context, StatusCodes.Status401Unauthorized, response);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Se produjo una excepción de negocio.");

            var response = ApiErrorResponse.Create(
                status: StatusCodes.Status409Conflict,
                error: ex.Message,
                path: context.Request.Path);

            await WriteResponseAsync(context, StatusCodes.Status409Conflict, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo una excepción no controlada.");

            var response = ApiErrorResponse.Create(
                status: StatusCodes.Status500InternalServerError,
                error: "Se produjo un error interno en el servidor.",
                path: context.Request.Path,
                details: new[] { "Intente nuevamente más tarde." });

            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, response);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, ApiErrorResponse response)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
