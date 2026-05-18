using Microsoft.EntityFrameworkCore;
using TravelDreams.MsReservas.DataAccess.Common;
using TravelDreams.MsReservas.DataAccess.Context;
using TravelDreams.MsReservas.DataAccess.Entities;
using TravelDreams.MsReservas.DataManagement.Interfaces;
using TravelDreams.MsReservas.DataManagement.Models;

namespace TravelDreams.MsReservas.DataManagement.Services;

public sealed class ClientesDataService : IClientesDataService
{
    private readonly ReservasDbContext _db;

    public ClientesDataService(ReservasDbContext db) => _db = db;

    public async Task<IReadOnlyList<ClienteDataModel>> ListarAsync(string? numeroIdentificacion, string? correo, string? estado, CancellationToken ct = default)
    {
        var query = _db.Clientes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(numeroIdentificacion)) query = query.Where(x => x.cli_numero_identificacion.Contains(numeroIdentificacion));
        if (!string.IsNullOrWhiteSpace(correo)) query = query.Where(x => x.cli_correo.Contains(correo));
        if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(x => x.cli_estado == estado);
        return await query.OrderBy(x => x.cli_numero_identificacion).Select(x => Map(x)).ToListAsync(ct);
    }

    public async Task<ClienteDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var entity = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.cli_guid == guid, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorUsuarioGuidAsync(Guid usuarioGuid, CancellationToken ct = default)
    {
        var entity = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.usu_guid == usuarioGuid, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken ct = default)
    {
        var entity = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.cli_numero_identificacion == numeroIdentificacion, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<ClienteDataModel> GuardarAsync(ClienteDataModel model, CancellationToken ct = default)
    {
        var entity = model.Guid.HasValue
            ? await _db.Clientes.FirstOrDefaultAsync(x => x.cli_guid == model.Guid, ct)
            : await _db.Clientes.FirstOrDefaultAsync(x => x.cli_numero_identificacion == model.NumeroIdentificacion, ct);

        if (entity is null)
        {
            entity = new ClienteEntity
            {
                usu_guid = model.UsuarioGuid,
                cli_tipo_identificacion = model.TipoIdentificacion,
                cli_numero_identificacion = model.NumeroIdentificacion,
                cli_usuario_ingreso = "ms-reservas",
                cli_ip_ingreso = "api"
            };
            _db.Clientes.Add(entity);
        }

        entity.usu_guid = model.UsuarioGuid ?? entity.usu_guid;
        entity.cli_tipo_identificacion = model.TipoIdentificacion;
        entity.cli_numero_identificacion = model.NumeroIdentificacion;
        entity.cli_nombres = model.Nombres;
        entity.cli_apellidos = model.Apellidos;
        entity.cli_razon_social = model.RazonSocial;
        entity.cli_correo = model.Correo;
        entity.cli_telefono = model.Telefono;
        entity.cli_direccion = model.Direccion;
        entity.cli_estado = DatabaseConstants.EstadoActivo;
        entity.cli_fecha_eliminacion = null;
        entity.cli_usuario_eliminacion = null;
        entity.cli_ip_eliminacion = null;

        await _db.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task<bool> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken ct = default)
    {
        var entity = await _db.Clientes.FirstOrDefaultAsync(x => x.cli_guid == guid, ct);
        if (entity is null) return false;

        entity.cli_estado = estado;
        if (estado == "I")
        {
            entity.cli_fecha_eliminacion = DateTime.UtcNow;
            entity.cli_usuario_eliminacion = usuario;
            entity.cli_ip_eliminacion = ip;
        }
        else
        {
            entity.cli_fecha_eliminacion = null;
            entity.cli_usuario_eliminacion = null;
            entity.cli_ip_eliminacion = null;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static ClienteDataModel Map(ClienteEntity entity) => new()
    {
        Guid = entity.cli_guid,
        UsuarioGuid = entity.usu_guid,
        TipoIdentificacion = entity.cli_tipo_identificacion,
        NumeroIdentificacion = entity.cli_numero_identificacion,
        Nombres = entity.cli_nombres,
        Apellidos = entity.cli_apellidos,
        RazonSocial = entity.cli_razon_social,
        Correo = entity.cli_correo,
        Telefono = entity.cli_telefono,
        Direccion = entity.cli_direccion,
        Estado = entity.cli_estado
    };
}
