using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TravelDreams.ApiGateway.Security;

public sealed class GatewayJwtValidator
{
    private readonly IConfiguration _configuration;

    public GatewayJwtValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtValidationResult Validate(string token)
    {
        var result = Validate(token, "JwtSettings");
        return result.Success ? result : Validate(token, "BookingAuth");
    }

    private JwtValidationResult Validate(string token, string sectionName)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return JwtValidationResult.Failed("Token ausente.");
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return JwtValidationResult.Failed("Token JWT mal formado.");
        }

        var secret = _configuration[$"{sectionName}:SecretKey"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            return JwtValidationResult.Failed($"{sectionName}:SecretKey no esta configurado en el gateway.");
        }

        try
        {
            var header = JsonSerializer.Deserialize<Dictionary<string, object>>(Base64UrlDecode(parts[0]));
            if (header is null ||
                !header.TryGetValue("alg", out var alg) ||
                !string.Equals(alg.ToString(), "HS256", StringComparison.OrdinalIgnoreCase))
            {
                return JwtValidationResult.Failed("Algoritmo JWT no soportado.");
            }

            var expectedSignature = Sign($"{parts[0]}.{parts[1]}", secret);
            if (!FixedTimeEquals(parts[2], expectedSignature))
            {
                return JwtValidationResult.Failed("Firma JWT invalida.");
            }

            using var payloadDocument = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            var payload = payloadDocument.RootElement;

            if (!ValidateStringClaim(payload, "iss", _configuration[$"{sectionName}:Issuer"]))
            {
                return JwtValidationResult.Failed("Issuer JWT invalido.");
            }

            if (!ValidateStringClaim(payload, "aud", _configuration[$"{sectionName}:Audience"]))
            {
                return JwtValidationResult.Failed("Audience JWT invalido.");
            }

            if (!payload.TryGetProperty("exp", out var expElement) ||
                !TryGetUnixSeconds(expElement, out var exp) ||
                DateTimeOffset.FromUnixTimeSeconds(exp) <= DateTimeOffset.UtcNow)
            {
                return JwtValidationResult.Failed("Token JWT expirado.");
            }

            if (!payload.TryGetProperty("sub", out var subElement))
            {
                return JwtValidationResult.Failed("Subject JWT invalido.");
            }

            var subject = subElement.GetString() ?? string.Empty;
            _ = Guid.TryParse(subject, out var userGuid);

            var login = payload.TryGetProperty("name", out var nameElement)
                ? nameElement.GetString() ?? subject
                : subject;

            var roles = ReadRoles(payload);
            return JwtValidationResult.Ok(new GatewayUser
            {
                UserGuid = userGuid,
                Login = login,
                Roles = roles
            });
        }
        catch
        {
            return JwtValidationResult.Failed("Token JWT invalido.");
        }
    }

    private static bool ValidateStringClaim(JsonElement payload, string claim, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected)) return true;
        return payload.TryGetProperty(claim, out var element) &&
               string.Equals(element.GetString(), expected, StringComparison.Ordinal);
    }

    private static IReadOnlyList<string> ReadRoles(JsonElement payload)
    {
        if (!payload.TryGetProperty("roles", out var rolesElement))
        {
            return [];
        }

        if (rolesElement.ValueKind == JsonValueKind.Array)
        {
            return rolesElement.EnumerateArray()
                .Select(x => x.GetString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .ToList();
        }

        if (rolesElement.ValueKind == JsonValueKind.String && rolesElement.GetString() is { } role)
        {
            return [role];
        }

        return [];
    }

    private static bool TryGetUnixSeconds(JsonElement element, out long value)
    {
        if (element.ValueKind == JsonValueKind.Number)
        {
            return element.TryGetInt64(out value);
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            return long.TryParse(element.GetString(), out value);
        }

        value = 0;
        return false;
    }

    private static string Sign(string signingInput, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
    }

    private static bool FixedTimeEquals(string actual, string expected)
    {
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        return actualBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var incoming = value.Replace('-', '+').Replace('_', '/');
        switch (incoming.Length % 4)
        {
            case 2:
                incoming += "==";
                break;
            case 3:
                incoming += "=";
                break;
        }

        return Convert.FromBase64String(incoming);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
