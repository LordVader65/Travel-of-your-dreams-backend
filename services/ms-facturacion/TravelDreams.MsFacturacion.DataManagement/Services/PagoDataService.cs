using Microsoft.EntityFrameworkCore;
using TravelDreams.MsFacturacion.DataAccess.Context;
using TravelDreams.MsFacturacion.DataAccess.Entities;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.DataManagement.Services;

public sealed class PagoDataService : IPagoDataService
{
    private readonly FacturacionDbContext _db;

    public PagoDataService(FacturacionDbContext db) => _db = db;

    public async Task<PagoDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var entity = await _db.Pagos.AsNoTracking()
            .Include(x => x.DatosFacturacion)
            .FirstOrDefaultAsync(x => x.pag_guid == guid, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<PagedResult<PagoDataModel>> ListarAsync(PagoFiltroDataModel filtro, CancellationToken ct = default)
    {
        var page = filtro.Page <= 0 ? 1 : filtro.Page;
        var limit = filtro.Limit <= 0 ? 20 : Math.Min(filtro.Limit, 100);
        var query = _db.Pagos.AsNoTracking().Include(x => x.DatosFacturacion).AsQueryable();

        if (filtro.ReservaGuid.HasValue) query = query.Where(x => x.rev_guid == filtro.ReservaGuid);
        if (filtro.ClienteGuid.HasValue) query = query.Where(x => x.cli_guid == filtro.ClienteGuid);
        if (!string.IsNullOrWhiteSpace(filtro.Metodo)) query = query.Where(x => x.pag_metodo == filtro.Metodo);
        if (!string.IsNullOrWhiteSpace(filtro.Estado)) query = query.Where(x => x.pag_estado == filtro.Estado);
        if (filtro.FechaDesdeUtc.HasValue) query = query.Where(x => x.pag_fecha_utc >= filtro.FechaDesdeUtc);
        if (filtro.FechaHastaUtc.HasValue) query = query.Where(x => x.pag_fecha_utc <= filtro.FechaHastaUtc);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.pag_fecha_utc)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return new PagedResult<PagoDataModel> { Items = items, Page = page, Limit = limit, Total = total };
    }

    internal static PagoDataModel Map(PagoEntity entity) => new()
    {
        Id = entity.pag_id,
        Guid = entity.pag_guid,
        ReservaGuid = entity.rev_guid,
        ClienteGuid = entity.cli_guid,
        DatosFacturacionGuid = entity.DatosFacturacion?.dfac_guid,
        Monto = entity.pag_monto,
        Moneda = entity.pag_moneda,
        Metodo = entity.pag_metodo,
        Referencia = entity.pag_referencia,
        FechaUtc = entity.pag_fecha_utc,
        OrigenCanal = entity.pag_origen_canal,
        Estado = entity.pag_estado,
        Observacion = entity.pag_observacion
    };
}
