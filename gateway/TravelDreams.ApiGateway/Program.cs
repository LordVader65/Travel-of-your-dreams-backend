using TravelDreams.ApiGateway;
using TravelDreams.ApiGateway.Audit;
using TravelDreams.ApiGateway.Configuration;
using TravelDreams.ApiGateway.Proxy;
using TravelDreams.ApiGateway.Security;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient("gateway-proxy");
builder.Services.AddHttpClient<AuditHttpClient>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var auditoriaUrl = configuration["Services:AuditoriaUrl"] ?? "http://localhost:5105";
    client.BaseAddress = new Uri(auditoriaUrl);
});
builder.Services.AddScoped<GatewayProxy>();
builder.Services.AddSingleton<GatewayJwtValidator>();
builder.Services.AddSingleton<BookingTokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapGet("/", (IConfiguration configuration) => Results.Ok(new
{
    service = "api-gateway",
    status = "running",
    auditLogging = configuration.GetValue("Gateway:EnableAuditLogging", true),
    downstream = new
    {
        identidad = configuration["Services:IdentidadUrl"],
        atracciones = configuration["Services:AtraccionesUrl"],
        reservas = configuration["Services:ReservasUrl"],
        facturacion = configuration["Services:FacturacionUrl"],
        auditoria = configuration["Services:AuditoriaUrl"]
    }
}));

app.MapPost("/api/v1/integrations/booking/token", (BookingTokenRequest request, BookingTokenService tokens) =>
{
    if (string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ClientSecret))
    {
        return Results.BadRequest(new { status = StatusCodes.Status400BadRequest, error = "ClientId y ClientSecret son obligatorios." });
    }

    if (!tokens.ValidateCredentials(request.ClientId, request.ClientSecret))
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new { status = StatusCodes.Status200OK, data = tokens.BuildTokenResponse() });
});

app.Map("/{**path}", async (HttpContext context, GatewayProxy proxy) =>
{
    var route = GatewayRoutes.All
        .Where(x => x.Matches(context.Request))
        .OrderByDescending(x => x.Prefixes.Max(prefix => prefix.Length))
        .ThenByDescending(x => x.RequiresAuthentication)
        .ThenBy(x => x.AllowedRoles.Length == 0 ? int.MaxValue : x.AllowedRoles.Length)
        .FirstOrDefault();

    if (route is null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new
        {
            status = StatusCodes.Status404NotFound,
            error = "Ruta no configurada en el API Gateway.",
            path = context.Request.Path.Value
        });
        return;
    }

    var authResult = AuthenticateIfRequired(context, route);
    if (!authResult) return;

    await proxy.ProxyAsync(context, route);
});

app.Run();

static bool AuthenticateIfRequired(HttpContext context, GatewayRoute route)
{
    if (!route.RequiresAuthentication && !route.ValidateTokenWhenPresent)
    {
        return true;
    }

    var authorization = context.Request.Headers.Authorization.ToString();
    var hasBearerToken = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

    if (!hasBearerToken && !route.RequiresAuthentication)
    {
        return true;
    }

    if (!hasBearerToken)
    {
        WriteAuthError(context, StatusCodes.Status401Unauthorized, "Falta token Bearer.");
        return false;
    }

    var validator = context.RequestServices.GetRequiredService<GatewayJwtValidator>();
    var result = validator.Validate(authorization["Bearer ".Length..].Trim());
    if (!result.Success || result.User is null)
    {
        WriteAuthError(context, StatusCodes.Status401Unauthorized, result.Error ?? "Token invalido.");
        return false;
    }

    if (route.AllowedRoles.Length > 0 &&
        !route.AllowedRoles.Any(role => result.User.Roles.Contains(role, StringComparer.OrdinalIgnoreCase)))
    {
        WriteAuthError(context, StatusCodes.Status403Forbidden, "El usuario no tiene permisos para esta ruta.");
        return false;
    }

    context.Items["GatewayUser"] = result.User;
    return true;
}

static void WriteAuthError(HttpContext context, int statusCode, string error)
{
    context.Response.StatusCode = statusCode;
    context.Response.WriteAsJsonAsync(new
    {
        status = statusCode,
        error,
        details = Array.Empty<string>(),
        timestamp = DateTime.UtcNow,
        path = context.Request.Path.Value
    }).GetAwaiter().GetResult();
}

public sealed record BookingTokenRequest(string ClientId, string ClientSecret);
