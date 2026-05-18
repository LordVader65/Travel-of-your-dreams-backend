namespace TravelDreams.ApiGateway.Security;

public sealed class GatewayUser
{
    public Guid UserGuid { get; init; }
    public string Login { get; init; } = string.Empty;
    public IReadOnlyList<string> Roles { get; init; } = [];
}
