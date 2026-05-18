using Microsoft.EntityFrameworkCore;
using TravelDreams.MsIdentidad.DataAccess.Common;
using TravelDreams.MsIdentidad.DataAccess.Context;
using TravelDreams.MsIdentidad.DataAccess.Entities;
using TravelDreams.MsIdentidad.DataManagement.Interfaces;
using TravelDreams.MsIdentidad.DataManagement.Models;

namespace TravelDreams.MsIdentidad.DataManagement.Services;

public sealed class IdentidadDataService : IIdentidadDataService
{
    private readonly IdentidadDbContext _db;

    public IdentidadDataService(IdentidadDbContext db) => _db = db;

    public async Task<IReadOnlyList<UsuarioDataModel>> ListarUsuariosAsync(CancellationToken ct = default) =>
        await UsuariosBase().OrderBy(x => x.usu_login).Select(x => MapUsuario(x)).ToListAsync(ct);

    public async Task<UsuarioDataModel?> ObtenerUsuarioPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var entity = await UsuariosBase().FirstOrDefaultAsync(x => x.usu_guid == guid, ct);
        return entity is null ? null : MapUsuario(entity);
    }

    public async Task<UsuarioDataModel?> ObtenerUsuarioPorLoginAsync(string login, CancellationToken ct = default)
    {
        var normalized = login.Trim().ToLowerInvariant();
        var entity = await UsuariosBase().FirstOrDefaultAsync(x => x.usu_login == normalized, ct);
        return entity is null ? null : MapUsuario(entity);
    }

    public async Task<UsuarioDataModel> CrearUsuarioAsync(string login, string passwordHash, IEnumerable<int> rolIds, string usuario, string ip, CancellationToken ct = default)
    {
        var normalized = login.Trim().ToLowerInvariant();
        var entity = new UsuarioEntity
        {
            usu_login = normalized,
            usu_password_hash = passwordHash,
            usu_usuario_registro = usuario,
            usu_ip_registro = ip,
            usu_estado = DatabaseConstants.EstadoActivo
        };
        _db.Usuarios.Add(entity);
        await _db.SaveChangesAsync(ct);

        foreach (var rolId in rolIds.Distinct())
        {
            _db.UsuarioRoles.Add(new UsuarioRolEntity { usu_id = entity.usu_id, rol_id = rolId, usu_rol_estado = DatabaseConstants.EstadoActivo });
        }
        await _db.SaveChangesAsync(ct);

        return await ObtenerUsuarioPorGuidAsync(entity.usu_guid, ct) ?? MapUsuario(entity);
    }

    public async Task<UsuarioDataModel?> CambiarEstadoUsuarioAsync(Guid guid, string estado, string usuario, string ip, CancellationToken ct = default)
    {
        var entity = await _db.Usuarios.FirstOrDefaultAsync(x => x.usu_guid == guid, ct);
        if (entity is null) return null;

        entity.usu_estado = estado;
        entity.usu_fecha_mod = DateTime.UtcNow;
        entity.usu_usuario_mod = usuario;
        entity.usu_ip_mod = ip;
        if (estado == DatabaseConstants.EstadoInactivo)
        {
            entity.usu_fecha_eliminacion = DateTime.UtcNow;
            entity.usu_usuario_eliminacion = usuario;
            entity.usu_ip_eliminacion = ip;
        }
        else
        {
            entity.usu_fecha_eliminacion = null;
            entity.usu_usuario_eliminacion = null;
            entity.usu_ip_eliminacion = null;
        }

        await _db.SaveChangesAsync(ct);
        return await ObtenerUsuarioPorGuidAsync(guid, ct);
    }

    public async Task<UsuarioDataModel?> CambiarPasswordAsync(Guid guid, string passwordHash, string usuario, string ip, CancellationToken ct = default)
    {
        var entity = await _db.Usuarios.FirstOrDefaultAsync(x => x.usu_guid == guid, ct);
        if (entity is null) return null;

        entity.usu_password_hash = passwordHash;
        entity.usu_fecha_mod = DateTime.UtcNow;
        entity.usu_usuario_mod = usuario;
        entity.usu_ip_mod = ip;
        await _db.SaveChangesAsync(ct);
        return await ObtenerUsuarioPorGuidAsync(guid, ct);
    }

    public async Task<bool> ReemplazarRolesAsync(Guid usuarioGuid, IEnumerable<int> rolIds, CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios.Include(x => x.UsuarioRoles).FirstOrDefaultAsync(x => x.usu_guid == usuarioGuid, ct);
        if (usuario is null) return false;

        var selected = rolIds.Where(x => x > 0).Distinct().ToHashSet();
        foreach (var existing in usuario.UsuarioRoles)
        {
            existing.usu_rol_estado = selected.Contains(existing.rol_id) ? DatabaseConstants.EstadoActivo : DatabaseConstants.EstadoInactivo;
            selected.Remove(existing.rol_id);
        }

        foreach (var rolId in selected)
        {
            usuario.UsuarioRoles.Add(new UsuarioRolEntity { rol_id = rolId, usu_rol_estado = DatabaseConstants.EstadoActivo });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<RolDataModel>> ListarRolesAsync(CancellationToken ct = default) =>
        await _db.Roles.AsNoTracking().OrderBy(x => x.rol_descripcion).Select(x => MapRol(x)).ToListAsync(ct);

    public async Task<RolDataModel?> ObtenerRolPorDescripcionAsync(string descripcion, CancellationToken ct = default)
    {
        var normalized = descripcion.Trim().ToUpperInvariant();
        var entity = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.rol_descripcion == normalized, ct);
        return entity is null ? null : MapRol(entity);
    }

    private IQueryable<UsuarioEntity> UsuariosBase() =>
        _db.Usuarios.AsNoTracking()
            .Include(x => x.UsuarioRoles.Where(ur => ur.usu_rol_estado == DatabaseConstants.EstadoActivo))
            .ThenInclude(x => x.Rol);

    private static UsuarioDataModel MapUsuario(UsuarioEntity entity) => new()
    {
        Id = entity.usu_id,
        Guid = entity.usu_guid,
        Login = entity.usu_login,
        PasswordHash = entity.usu_password_hash,
        Estado = entity.usu_estado,
        Roles = entity.UsuarioRoles
            .Where(x => x.usu_rol_estado == DatabaseConstants.EstadoActivo && x.Rol is not null && x.Rol.rol_estado == DatabaseConstants.EstadoActivo)
            .Select(x => MapRol(x.Rol!))
            .ToList()
    };

    private static RolDataModel MapRol(RolEntity entity) => new()
    {
        Id = entity.rol_id,
        Guid = entity.rol_guid,
        Descripcion = entity.rol_descripcion,
        Estado = entity.rol_estado
    };
}
