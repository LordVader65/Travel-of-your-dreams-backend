using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class PagoQueryRepository : IPagoQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public PagoQueryRepository(AtraccionesDbContext context) => _context = context;

    public async Task<PagedResult<PagoEntity>> ListarPorReservaAsync(Guid reservaGuid, int page, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.Pagos.AsNoTracking()
            .Include(x => x.Reserva)
            .Where(x => x.Reserva != null && x.Reserva.rev_guid == reservaGuid)
            .OrderByDescending(x => x.pag_fecha_utc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<PagoEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public async Task<PagedResult<PagoEntity>> ListarAsync(
        Guid? reservaGuid,
        Guid? clienteGuid,
        string? metodo,
        string? estado,
        DateTime? fechaDesdeUtc,
        DateTime? fechaHastaUtc,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Pagos.AsNoTracking()
            .Include(x => x.Reserva).ThenInclude(x => x!.Cliente)
            .AsQueryable();

        if (reservaGuid.HasValue)
            query = query.Where(x => x.Reserva != null && x.Reserva.rev_guid == reservaGuid.Value);
        if (clienteGuid.HasValue)
            query = query.Where(x => x.Reserva != null && x.Reserva.Cliente != null && x.Reserva.Cliente.cli_guid == clienteGuid.Value);
        if (!string.IsNullOrWhiteSpace(metodo))
            query = query.Where(x => x.pag_metodo == metodo);
        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.pag_estado == estado);
        if (fechaDesdeUtc.HasValue)
            query = query.Where(x => x.pag_fecha_utc >= fechaDesdeUtc.Value);
        if (fechaHastaUtc.HasValue)
            query = query.Where(x => x.pag_fecha_utc <= fechaHastaUtc.Value);

        query = query.OrderByDescending(x => x.pag_fecha_utc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<PagoEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }
}
