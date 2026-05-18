using Microsoft.EntityFrameworkCore;
using TravelDreams.MsAuditoria.DataAccess.Context;
using TravelDreams.MsAuditoria.DataAccess.Entities;
using TravelDreams.MsAuditoria.DataManagement.Interfaces;
using TravelDreams.MsAuditoria.DataManagement.Models;

namespace TravelDreams.MsAuditoria.DataManagement.Services;

public sealed class AuditoriaLogDataService : IAuditoriaLogDataService
{
    private readonly AuditoriaDbContext _db;

    public AuditoriaLogDataService(AuditoriaDbContext db) => _db = db;

    public async Task<PagedResult<AuditoriaLogDataModel>> ConsultarAsync(AuditoriaFiltroDataModel filtro, CancellationToken ct = default)
    {
        var page = filtro.Page <= 0 ? 1 : filtro.Page;
        var limit = filtro.Limit <= 0 ? 20 : Math.Min(filtro.Limit, 100);
        var query = _db.AuditoriaLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Servicio)) query = query.Where(x => x.log_servicio == filtro.Servicio);
        if (!string.IsNullOrWhiteSpace(filtro.Tabla)) query = query.Where(x => x.log_tabla == filtro.Tabla);
        if (!string.IsNullOrWhiteSpace(filtro.Operacion)) query = query.Where(x => x.log_operacion == filtro.Operacion);
        if (!string.IsNullOrWhiteSpace(filtro.Usuario)) query = query.Where(x => x.log_usuario.Contains(filtro.Usuario));
        if (!string.IsNullOrWhiteSpace(filtro.CorrelationId)) query = query.Where(x => x.log_correlation_id == filtro.CorrelationId);
        if (filtro.DesdeUtc.HasValue) query = query.Where(x => x.log_fecha_utc >= filtro.DesdeUtc.Value);
        if (filtro.HastaUtc.HasValue) query = query.Where(x => x.log_fecha_utc <= filtro.HastaUtc.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.log_fecha_utc)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return new PagedResult<AuditoriaLogDataModel> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public async Task<IReadOnlyList<AuditoriaLogDataModel>> ConsultarPorTablaAsync(string tabla, CancellationToken ct = default) =>
        await _db.AuditoriaLogs.AsNoTracking()
            .Where(x => x.log_tabla == tabla)
            .OrderByDescending(x => x.log_fecha_utc)
            .Select(x => Map(x))
            .ToListAsync(ct);

    public async Task<AuditoriaLogDataModel?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var entity = await _db.AuditoriaLogs.AsNoTracking().FirstOrDefaultAsync(x => x.log_guid == guid, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<long> RegistrarAsync(AuditoriaLogDataModel model, CancellationToken ct = default)
    {
        if (model.EventoId.HasValue && await EventoProcesadoAsync(model.EventoId.Value, ct))
        {
            return 0;
        }

        var entity = new AuditoriaLogEntity
        {
            log_servicio = model.Servicio,
            log_tabla = model.Tabla,
            log_operacion = model.Operacion,
            log_registro_id = model.RegistroId,
            log_registro_guid = model.RegistroGuid,
            log_datos_anteriores = model.DatosAnteriores,
            log_datos_nuevos = model.DatosNuevos,
            log_fecha_utc = model.FechaUtc == default ? DateTime.UtcNow : model.FechaUtc,
            log_usuario = model.Usuario,
            log_ip = model.Ip,
            log_origen_canal = model.OrigenCanal,
            log_correlation_id = model.CorrelationId,
            evento_id = model.EventoId
        };

        _db.AuditoriaLogs.Add(entity);
        if (model.EventoId.HasValue)
        {
            _db.EventosProcesados.Add(new EventoProcesadoEntity
            {
                ep_evento_id = model.EventoId.Value,
                ep_tipo = $"{model.Servicio}.{model.Tabla}.{model.Operacion}",
                ep_origen_servicio = model.Servicio,
                ep_fecha_procesado_utc = DateTime.UtcNow,
                ep_correlation_id = model.CorrelationId
            });
        }

        await _db.SaveChangesAsync(ct);
        return entity.log_id;
    }

    public Task<bool> EventoProcesadoAsync(Guid eventoId, CancellationToken ct = default) =>
        _db.EventosProcesados.AsNoTracking().AnyAsync(x => x.ep_evento_id == eventoId, ct);

    private static AuditoriaLogDataModel Map(AuditoriaLogEntity entity) => new()
    {
        Id = entity.log_id,
        Guid = entity.log_guid,
        Servicio = entity.log_servicio,
        Tabla = entity.log_tabla,
        Operacion = entity.log_operacion,
        RegistroId = entity.log_registro_id,
        RegistroGuid = entity.log_registro_guid,
        DatosAnteriores = entity.log_datos_anteriores,
        DatosNuevos = entity.log_datos_nuevos,
        FechaUtc = entity.log_fecha_utc,
        Usuario = entity.log_usuario,
        Ip = entity.log_ip,
        OrigenCanal = entity.log_origen_canal,
        CorrelationId = entity.log_correlation_id,
        EventoId = entity.evento_id
    };
}
