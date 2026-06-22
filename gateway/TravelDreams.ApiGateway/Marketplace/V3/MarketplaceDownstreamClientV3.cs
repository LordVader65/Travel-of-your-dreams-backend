using System.Text.Json;
using TravelDreams.ApiGateway.Security;

namespace TravelDreams.ApiGateway.Marketplace.V3;

public sealed class MarketplaceDownstreamClientV3
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _configuration;
    private readonly GatewayJwtValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MarketplaceDownstreamClientV3(
        IHttpClientFactory clients,
        IConfiguration configuration,
        GatewayJwtValidator validator,
        IHttpContextAccessor httpContextAccessor)
    {
        _clients = clients;
        _configuration = configuration;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<MarketplaceUserContext> RequireClientAsync(CancellationToken ct)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw GraphQlError("AUTH_REQUIRED", "No existe contexto HTTP.");
        var authorization = httpContext.Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw GraphQlError("AUTH_REQUIRED", "Falta token Bearer.");

        var validation = _validator.Validate(authorization["Bearer ".Length..].Trim());
        if (!validation.Success || validation.User is null)
            throw GraphQlError("AUTH_INVALID", validation.Error ?? "Token invalido.");
        if (!validation.User.Roles.Contains("CLIENTE", StringComparer.OrdinalIgnoreCase))
            throw GraphQlError("FORBIDDEN", "GraphQL V3 esta disponible para clientes internos.");

        var profile = await SendRawAsync("ReservasUrl", HttpMethod.Get, "/api/v1/me", validation.User, ct);
        if (!profile.HasValue ||
            !profile.Value.TryGetProperty("data", out var profileData) ||
            ReadGuid(profileData, "guid") is not { } clienteGuid ||
            clienteGuid == Guid.Empty)
            throw GraphQlError("CLIENT_NOT_FOUND", "No existe un perfil de cliente vinculado al usuario.");

        return new MarketplaceUserContext(validation.User.UserGuid, clienteGuid, validation.User.Roles);
    }

    public Task<object?> GetPublicAsync(string serviceKey, string path, CancellationToken ct) =>
        SendAsync(serviceKey, HttpMethod.Get, path, null, ct);

    public Task<object?> GetClientAsync(string serviceKey, string path, MarketplaceUserContext user, CancellationToken ct) =>
        SendAsync(serviceKey, HttpMethod.Get, path, new GatewayUser
        {
            UserGuid = user.UserGuid,
            Roles = user.Roles
        }, ct);

    public async Task<EstadoReservaProcesoResponse> GetReservaStatusAsync(Guid correlationId, MarketplaceUserContext user, CancellationToken ct)
    {
        var result = await SendRawAsync("ReservasUrl", HttpMethod.Get,
            $"/internal/v3/marketplace/reservas/solicitudes/{correlationId}?clienteGuid={user.ClienteGuid}", null, ct, allowNotFound: true);
        if (result is null) return new(correlationId, "RECIBIDA", null, null, null, null);
        var data = result.Value.GetProperty("data");
        return new(
            correlationId,
            ReadString(data, "estado") ?? "RECIBIDA",
            ReadGuid(data, "reservaGuid"),
            ReadString(data, "reservaCodigo"),
            ReadString(data, "error"),
            ReadDateTime(data, "updatedAtUtc"));
    }

    public async Task<EstadoPagoProcesoResponse> GetPagoStatusAsync(Guid correlationId, MarketplaceUserContext user, CancellationToken ct)
    {
        var result = await SendRawAsync("FacturacionUrl", HttpMethod.Get,
            $"/internal/v3/marketplace/pagos/solicitudes/{correlationId}?clienteGuid={user.ClienteGuid}", null, ct, allowNotFound: true);
        if (result is null) return new(correlationId, "RECIBIDA", null, null, null, null, null);
        var data = result.Value.GetProperty("data");
        return new(
            correlationId,
            ReadString(data, "estado") ?? "RECIBIDA",
            ReadGuid(data, "reservaGuid"),
            ReadGuid(data, "facturaGuid"),
            ReadString(data, "facturaNumero"),
            ReadString(data, "error"),
            ReadDateTime(data, "updatedAtUtc"));
    }

    private async Task<object?> SendAsync(string serviceKey, HttpMethod method, string path, GatewayUser? user, CancellationToken ct)
    {
        var document = await SendRawAsync(serviceKey, method, path, user, ct);
        return document.HasValue ? document.Value : null;
    }

    private async Task<JsonElement?> SendRawAsync(
        string serviceKey,
        HttpMethod method,
        string path,
        GatewayUser? user,
        CancellationToken ct,
        bool allowNotFound = false)
    {
        var baseUrl = _configuration[$"Services:{serviceKey}"]
            ?? throw GraphQlError("CONFIGURATION_ERROR", $"Services:{serviceKey} no esta configurado.");
        using var request = new HttpRequestMessage(method, new Uri(new Uri(baseUrl.TrimEnd('/') + "/"), path.TrimStart('/')));
        if (user is not null)
        {
            request.Headers.TryAddWithoutValidation("X-User-Guid", user.UserGuid.ToString());
            request.Headers.TryAddWithoutValidation("X-Roles", string.Join(',', user.Roles));
        }

        var response = await _clients.CreateClient("marketplace-v3").SendAsync(request, ct);
        if (allowNotFound && response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw GraphQlError("DOWNSTREAM_ERROR", ExtractError(json) ?? $"Servicio {serviceKey} respondio {(int)response.StatusCode}.");

        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    private static string? ExtractError(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return ReadString(document.RootElement, "error") ?? ReadString(document.RootElement, "message");
        }
        catch { return null; }
    }

    private static string? ReadString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static Guid? ReadGuid(JsonElement element, string name) =>
        ReadString(element, name) is { } value && Guid.TryParse(value, out var guid) ? guid : null;

    private static DateTime? ReadDateTime(JsonElement element, string name) =>
        ReadString(element, name) is { } value && DateTime.TryParse(value, out var date) ? date : null;

    private static GraphQLException GraphQlError(string code, string message) =>
        new(ErrorBuilder.New().SetCode(code).SetMessage(message).Build());
}
