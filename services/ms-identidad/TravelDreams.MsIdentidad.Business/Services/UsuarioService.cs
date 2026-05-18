using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;
using TravelDreams.MsIdentidad.DataAccess.Common;
using TravelDreams.MsIdentidad.DataManagement.Interfaces;

namespace TravelDreams.MsIdentidad.Business.Services;

public sealed class UsuarioService : IUsuarioService
{
    private const string SeedAdminLogin = "admin@travelofyourdreams.local";
    private readonly IIdentidadDataService _data;

    public UsuarioService(IIdentidadDataService data) => _data = data;

    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken ct = default) =>
        (await _data.ListarUsuariosAsync(ct)).Select(IdentidadMappers.Usuario).ToList();

    public async Task<UsuarioResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var usuario = await _data.ObtenerUsuarioPorGuidAsync(guid, ct);
        return usuario is null ? null : IdentidadMappers.Usuario(usuario);
    }

    public async Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CancellationToken ct = default)
    {
        ValidateCreate(request);
        if (await _data.ObtenerUsuarioPorLoginAsync(request.Login, ct) is not null)
        {
            throw new InvalidOperationException("El usuario ya existe.");
        }

        var rolIds = request.RolIds.Where(x => x > 0).Distinct().ToList();
        if (rolIds.Count == 0 && await _data.ObtenerRolPorDescripcionAsync(DatabaseConstants.RolCliente, ct) is { } rolCliente)
        {
            rolIds.Add(rolCliente.Id);
        }

        return IdentidadMappers.Usuario(await _data.CrearUsuarioAsync(request.Login, PasswordHasher.Hash(request.Password), rolIds, "admin", "api", ct));
    }

    public async Task<UsuarioResponse?> CambiarEstadoAsync(Guid guid, CambiarEstadoUsuarioRequest request, CancellationToken ct = default)
    {
        if (request.Estado is not ("A" or "I")) throw new InvalidOperationException("Estado no permitido.");
        var usuario = await _data.ObtenerUsuarioPorGuidAsync(guid, ct) ?? throw new InvalidOperationException("Usuario no encontrado.");
        EnsureNotSeedAdmin(usuario.Login);

        if (request.Estado == DatabaseConstants.EstadoInactivo && usuario.Roles.Any(x => x.Descripcion == DatabaseConstants.RolAdmin))
        {
            var adminsActivos = (await _data.ListarUsuariosAsync(ct)).Count(x => x.Estado == DatabaseConstants.EstadoActivo && x.Roles.Any(r => r.Descripcion == DatabaseConstants.RolAdmin));
            if (adminsActivos <= 1) throw new InvalidOperationException("Debe existir al menos un usuario administrador activo.");
        }

        var updated = await _data.CambiarEstadoUsuarioAsync(guid, request.Estado, "admin", "api", ct);
        return updated is null ? null : IdentidadMappers.Usuario(updated);
    }

    public async Task<UsuarioResponse?> CambiarPasswordAsync(Guid guid, CambiarPasswordRequest request, bool validarActual, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.PasswordNueva) || request.PasswordNueva.Length < 8)
        {
            throw new InvalidOperationException("PasswordNueva debe tener al menos 8 caracteres.");
        }

        var usuario = await _data.ObtenerUsuarioPorGuidAsync(guid, ct) ?? throw new InvalidOperationException("Usuario no encontrado.");
        if (validarActual && !PasswordHasher.Matches(request.PasswordActual ?? string.Empty, usuario.PasswordHash))
        {
            throw new InvalidOperationException("La contrasena actual no es valida.");
        }

        var updated = await _data.CambiarPasswordAsync(guid, PasswordHasher.Hash(request.PasswordNueva), "admin", "api", ct);
        return updated is null ? null : IdentidadMappers.Usuario(updated);
    }

    public async Task<bool> CambiarRolesAsync(Guid guid, CambiarRolesRequest request, CancellationToken ct = default)
    {
        if (request.RolIds.Count == 0 || request.RolIds.Any(x => x <= 0)) throw new InvalidOperationException("RolIds es obligatorio.");
        var usuario = await _data.ObtenerUsuarioPorGuidAsync(guid, ct) ?? throw new InvalidOperationException("Usuario no encontrado.");
        EnsureNotSeedAdmin(usuario.Login);
        return await _data.ReemplazarRolesAsync(guid, request.RolIds, ct);
    }

    public async Task<IReadOnlyList<RolResponse>> ListarRolesAsync(CancellationToken ct = default) =>
        (await _data.ListarRolesAsync(ct)).Select(IdentidadMappers.Rol).ToList();

    private static void ValidateCreate(CrearUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || !request.Login.Contains('@')) throw new InvalidOperationException("Login debe ser un correo valido.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8) throw new InvalidOperationException("Password debe tener al menos 8 caracteres.");
        request.Login = request.Login.Trim().ToLowerInvariant();
    }

    private static void EnsureNotSeedAdmin(string login)
    {
        if (string.Equals(login, SeedAdminLogin, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El usuario administrador principal no puede desactivarse ni cambiar de rol.");
        }
    }
}
