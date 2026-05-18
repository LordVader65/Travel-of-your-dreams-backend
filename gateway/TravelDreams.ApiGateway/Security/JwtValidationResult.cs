namespace TravelDreams.ApiGateway.Security;

public sealed class JwtValidationResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public GatewayUser? User { get; init; }

    public static JwtValidationResult Failed(string error) => new() { Error = error };
    public static JwtValidationResult Ok(GatewayUser user) => new() { Success = true, User = user };
}
