namespace TravelDreams.ApiGateway;

public sealed class GatewayRoute
{
    public string Name { get; init; } = string.Empty;
    public string ServiceKey { get; init; } = string.Empty;
    public string[] Methods { get; init; } = [];
    public string[] Prefixes { get; init; } = [];
    public string[] Contains { get; init; } = [];
    public bool RequiresAuthentication { get; init; }
    public bool ValidateTokenWhenPresent { get; init; }
    public string[] AllowedRoles { get; init; } = [];

    public bool Matches(HttpRequest request)
    {
        if (Methods.Length > 0 && !Methods.Any(x => x.Equals(request.Method, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var value = request.Path.Value ?? string.Empty;

        if (Prefixes.Any(prefix => value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            return Contains.Length == 0 || Contains.Any(value.Contains);
        }

        return false;
    }
}
