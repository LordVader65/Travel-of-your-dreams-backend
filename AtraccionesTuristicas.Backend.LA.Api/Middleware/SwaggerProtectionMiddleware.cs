namespace AtraccionesTuristicas.Backend.LA.Api.Middleware;

public sealed class SwaggerProtectionMiddleware
{
    private const string SwaggerApiKeyHeader = "X-Swagger-Key";

    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public SwaggerProtectionMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _next = next;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (_environment.IsDevelopment())
        {
            await _next(context);
            return;
        }

        var requireApiKey = _configuration.GetValue("Swagger:RequireApiKey", true);
        if (!requireApiKey)
        {
            await _next(context);
            return;
        }

        var expectedKey = _configuration["Swagger:ApiKey"];
        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        if (!context.Request.Headers.TryGetValue(SwaggerApiKeyHeader, out var providedKey) ||
            string.IsNullOrWhiteSpace(providedKey) ||
            !string.Equals(providedKey.ToString(), expectedKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
