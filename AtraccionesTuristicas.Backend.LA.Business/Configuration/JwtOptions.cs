namespace AtraccionesTuristicas.Backend.LA.Business.Configuration;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpirationMinutes { get; set; } = 60;
}
