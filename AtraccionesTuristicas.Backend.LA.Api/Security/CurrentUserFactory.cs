using AtraccionesTuristicas.Backend.LA.Business.Common;

namespace AtraccionesTuristicas.Backend.LA.Api.Security;

public interface ICurrentUserFactory
{
    CurrentUserData Create(HttpContext context);
}

public sealed class CurrentUserFactory : ICurrentUserFactory
{
    public CurrentUserData Create(HttpContext context) => new()
    {
        UsuarioGuid = context.User.GetUsuarioGuid(),
        ClienteGuid = context.User.GetClienteGuid(),
        Login = context.User.Identity?.Name ?? context.User.FindFirst("name")?.Value ?? "anonymous",
        Roles = context.User.GetRoles(),
        Ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
    };
}
