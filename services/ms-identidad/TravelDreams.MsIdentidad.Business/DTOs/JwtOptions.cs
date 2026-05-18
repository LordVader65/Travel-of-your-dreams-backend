namespace TravelDreams.MsIdentidad.Business.DTOs;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "TravelDreams";
    public string Audience { get; set; } = "TravelDreams.Clients";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 120;
}
