using System.Security.Claims;

namespace AtraccionesTuristicas.Backend.LA.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUsuarioGuid(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"), out var guid) ? guid : null;

    public static Guid? GetClienteGuid(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue("cliente_guid"), out var guid) ? guid : null;

    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal user) =>
        user.FindAll(ClaimTypes.Role).Concat(user.FindAll("roles")).Select(x => x.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
}
