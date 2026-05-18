using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.DataManagement.Models;

namespace TravelDreams.MsIdentidad.Business.Services;

internal sealed class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(JwtOptions options) => _options = options;

    public string BuildToken(UsuarioDataModel usuario, DateTime expires)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = new DateTimeOffset(expires).ToUnixTimeSeconds();
        var roles = usuario.Roles.Select(x => x.Descripcion).ToList();
        var header = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object> { ["alg"] = "HS256", ["typ"] = "JWT" }));
        var payload = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object?>
        {
            ["sub"] = usuario.Guid.ToString(),
            ["name"] = usuario.Login,
            ["roles"] = roles,
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

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
