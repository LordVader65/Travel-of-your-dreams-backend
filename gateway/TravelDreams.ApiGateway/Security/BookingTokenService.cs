using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TravelDreams.ApiGateway.Security;

public sealed class BookingTokenService
{
    private readonly BookingAuthOptions _options;

    public BookingTokenService(IConfiguration configuration)
    {
        _options = configuration.GetSection("BookingAuth").Get<BookingAuthOptions>() ?? new BookingAuthOptions();
    }

    public bool ValidateCredentials(string clientId, string clientSecret) =>
        !string.IsNullOrWhiteSpace(_options.ClientId) &&
        !string.IsNullOrWhiteSpace(_options.ClientSecret) &&
        !string.IsNullOrWhiteSpace(_options.SecretKey) &&
        FixedTimeEquals(clientId, _options.ClientId) && FixedTimeEquals(clientSecret, _options.ClientSecret);

    public object BuildTokenResponse()
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.ExpirationMinutes);
        return new
        {
            accessToken = BuildToken(expires),
            tokenType = "Bearer",
            expiresIn = _options.ExpirationMinutes * 60
        };
    }

    private string BuildToken(DateTimeOffset expires)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = expires.ToUnixTimeSeconds();
        var header = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object> { ["alg"] = "HS256", ["typ"] = "JWT" }));
        var payload = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object?>
        {
            ["sub"] = _options.ClientId,
            ["name"] = "Booking External Integration",
            ["roles"] = new[] { "BOOKING_INTEGRATION" },
            ["scope"] = new[] { "booking:reservas:read", "booking:reservas:write", "booking:pagos:write", "booking:facturas:read" },
            ["iss"] = _options.Issuer,
            ["aud"] = _options.Audience,
            ["iat"] = now,
            ["exp"] = exp
        }));
        var signingInput = $"{header}.{payload}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.SecretKey));
        var signature = Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
        return $"{signingInput}.{signature}";
    }

    private static bool FixedTimeEquals(string actual, string expected)
    {
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        return actualBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
