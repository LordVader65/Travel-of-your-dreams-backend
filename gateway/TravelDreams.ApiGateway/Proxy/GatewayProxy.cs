using System.Net.Http.Headers;
using System.Text.Json;
using TravelDreams.ApiGateway.Audit;
using TravelDreams.ApiGateway.Security;

namespace TravelDreams.ApiGateway.Proxy;

public sealed class GatewayProxy
{
    private static readonly HashSet<string> SkippedRequestHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Host",
        "Connection",
        "Content-Length",
        "Transfer-Encoding",
        "Keep-Alive",
        "Upgrade",
        "Proxy-Connection"
    };

    private static readonly HashSet<string> SkippedResponseHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Transfer-Encoding",
        "Connection",
        "Keep-Alive",
        "Upgrade",
        "Proxy-Connection"
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly AuditHttpClient _audit;
    private readonly ILogger<GatewayProxy> _logger;

    public GatewayProxy(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        AuditHttpClient audit,
        ILogger<GatewayProxy> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _audit = audit;
        _logger = logger;
    }

    public async Task ProxyAsync(HttpContext context, GatewayRoute route)
    {
        var downstreamBaseUrl = _configuration[$"Services:{route.ServiceKey}"];
        if (string.IsNullOrWhiteSpace(downstreamBaseUrl))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new
            {
                status = StatusCodes.Status503ServiceUnavailable,
                error = $"No se configuro Services:{route.ServiceKey}."
            });
            return;
        }

        var downstreamUri = BuildDownstreamUri(downstreamBaseUrl, context.Request);
        var requestMessage = await CreateProxyRequestAsync(context, downstreamUri);

        var startedAt = DateTime.UtcNow;
        using var responseMessage = await _httpClientFactory.CreateClient("gateway-proxy")
            .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

        await CopyResponseAsync(context, responseMessage);
        await AuditAsync(context, route, downstreamUri, startedAt);
    }

    private static Uri BuildDownstreamUri(string baseUrl, HttpRequest request)
    {
        var builder = new UriBuilder(baseUrl)
        {
            Path = CombinePath(new Uri(baseUrl).AbsolutePath, request.Path),
            Query = request.QueryString.HasValue ? request.QueryString.Value![1..] : string.Empty
        };

        return builder.Uri;
    }

    private static string CombinePath(string basePath, PathString path)
    {
        var normalizedBase = basePath.TrimEnd('/');
        var normalizedPath = (path.Value ?? string.Empty).TrimStart('/');
        return string.IsNullOrWhiteSpace(normalizedBase)
            ? normalizedPath
            : $"{normalizedBase}/{normalizedPath}";
    }

    private static async Task<HttpRequestMessage> CreateProxyRequestAsync(HttpContext context, Uri downstreamUri)
    {
        var request = context.Request;
        var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), downstreamUri);

        if (!HttpMethods.IsGet(request.Method) &&
            !HttpMethods.IsHead(request.Method) &&
            !HttpMethods.IsDelete(request.Method) &&
            request.ContentLength is > 0)
        {
            requestMessage.Content = new StreamContent(request.Body);
            if (!string.IsNullOrWhiteSpace(request.ContentType))
            {
                requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
            }
        }

        foreach (var header in request.Headers)
        {
            if (SkippedRequestHeaders.Contains(header.Key)) continue;

            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        if (!request.Headers.ContainsKey("X-Correlation-ID"))
        {
            requestMessage.Headers.TryAddWithoutValidation("X-Correlation-ID", context.TraceIdentifier);
        }

        if (context.Items.TryGetValue("GatewayUser", out var item) && item is GatewayUser user)
        {
            requestMessage.Headers.TryAddWithoutValidation("X-User-Guid", user.UserGuid.ToString());
            requestMessage.Headers.TryAddWithoutValidation("X-User", user.Login);
            requestMessage.Headers.TryAddWithoutValidation("X-Roles", string.Join(',', user.Roles));
        }

        await Task.CompletedTask;
        return requestMessage;
    }

    private static async Task CopyResponseAsync(HttpContext context, HttpResponseMessage responseMessage)
    {
        context.Response.StatusCode = (int)responseMessage.StatusCode;

        foreach (var header in responseMessage.Headers)
        {
            if (!SkippedResponseHeaders.Contains(header.Key))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        foreach (var header in responseMessage.Content.Headers)
        {
            if (!SkippedResponseHeaders.Contains(header.Key))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        await responseMessage.Content.CopyToAsync(context.Response.Body);
    }

    private async Task AuditAsync(HttpContext context, GatewayRoute route, Uri downstreamUri, DateTime startedAt)
    {
        if (!_configuration.GetValue("Gateway:EnableAuditLogging", true)) return;
        if (route.Name.Equals("auditoria", StringComparison.OrdinalIgnoreCase)) return;

        var request = context.Request;
        var correlationId = request.Headers.TryGetValue("X-Correlation-ID", out var value)
            ? value.ToString()
            : context.TraceIdentifier;

        var payload = JsonSerializer.Serialize(new
        {
            route = route.Name,
            method = request.Method,
            path = request.Path.Value,
            query = request.QueryString.Value,
            downstream = downstreamUri.GetLeftPart(UriPartial.Path),
            statusCode = context.Response.StatusCode,
            elapsedMs = (int)(DateTime.UtcNow - startedAt).TotalMilliseconds
        });

        var user = context.Items.TryGetValue("GatewayUser", out var item) && item is GatewayUser gatewayUser
            ? gatewayUser.Login
            : "anonymous";

        await _audit.TryPublishAsync(new AuditEvent
        {
            Servicio = "api-gateway",
            Tabla = route.Name,
            Operacion = "BUSINESS_EVENT",
            DatosNuevos = payload,
            Usuario = user,
            Ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            CorrelationId = correlationId
        }, context.RequestAborted);

        _logger.LogInformation(
            "Proxied {Method} {Path} to {Route} with {StatusCode}",
            request.Method,
            request.Path.Value,
            route.Name,
            context.Response.StatusCode);
    }
}
