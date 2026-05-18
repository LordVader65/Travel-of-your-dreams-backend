using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.DataAccess.Common;
using TravelDreams.MsFacturacion.DataAccess.Context;
using TravelDreams.MsFacturacion.DataAccess.Entities;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.DataManagement.Services;

public sealed class DatosFacturacionDataService : IDatosFacturacionDataService
{
    private readonly FacturacionDbContext _db;

    public DatosFacturacionDataService(FacturacionDbContext db) => _db = db;

    public async Task<IReadOnlyList<DatosFacturacionDataModel>> ListarActivosPorClienteAsync(Guid clienteGuid, CancellationToken ct = default) =>
        await _db.DatosFacturacion.AsNoTracking()
            .Where(x => x.cli_guid == clienteGuid && x.dfac_estado == DatabaseConstants.EstadoActivo)
            .OrderBy(x => x.dfac_numero_identificacion)
            .Select(x => Map(x))
            .ToListAsync(ct);

    public async Task<DatosFacturacionDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var entity = await _db.DatosFacturacion.AsNoTracking().FirstOrDefaultAsync(x => x.dfac_guid == guid, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<DatosFacturacionDataModel> GuardarAsync(DatosFacturacionDataModel model, string usuario, string ip, CancellationToken ct = default)
    {
        var entity = model.Guid == Guid.Empty
            ? null
            : await _db.DatosFacturacion.FirstOrDefaultAsync(x => x.dfac_guid == model.Guid, ct);

        if (entity is null)
        {
            entity = new DatosFacturacionEntity
            {
                cli_guid = model.ClienteGuid,
                dfac_usuario_ingreso = usuario,
                dfac_ip_ingreso = ip
            };
            _db.DatosFacturacion.Add(entity);
        }
        else
        {
            entity.dfac_fecha_mod = DateTime.UtcNow;
            entity.dfac_usuario_mod = usuario;
            entity.dfac_ip_mod = ip;
        }

        entity.cli_guid = model.ClienteGuid;
        entity.dfac_tipo_identificacion = model.TipoIdentificacion;
        entity.dfac_numero_identificacion = model.NumeroIdentificacion;
        entity.dfac_razon_social = model.RazonSocial;
        entity.dfac_nombre = model.Nombre;
        entity.dfac_apellido = model.Apellido;
        entity.dfac_correo = model.Correo;
        entity.dfac_telefono = model.Telefono;
        entity.dfac_direccion = model.Direccion;
        entity.dfac_estado = DatabaseConstants.EstadoActivo;

        await _db.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task<bool> InactivarAsync(Guid guid, string usuario, string ip, CancellationToken ct = default)
    {
        var entity = await _db.DatosFacturacion.FirstOrDefaultAsync(x => x.dfac_guid == guid, ct);
        if (entity is null) return false;

        entity.dfac_estado = DatabaseConstants.EstadoInactivo;
        entity.dfac_fecha_eliminacion = DateTime.UtcNow;
        entity.dfac_usuario_eliminacion = usuario;
        entity.dfac_ip_eliminacion = ip;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    internal static DatosFacturacionDataModel Map(DatosFacturacionEntity entity) => new()
    {
        Id = entity.dfac_id,
        Guid = entity.dfac_guid,
        ClienteGuid = entity.cli_guid,
        TipoIdentificacion = entity.dfac_tipo_identificacion,
        NumeroIdentificacion = entity.dfac_numero_identificacion,
        RazonSocial = entity.dfac_razon_social,
        Nombre = entity.dfac_nombre,
        Apellido = entity.dfac_apellido,
        Correo = entity.dfac_correo,
        Telefono = entity.dfac_telefono,
        Direccion = entity.dfac_direccion,
        Estado = entity.dfac_estado
    };
}
