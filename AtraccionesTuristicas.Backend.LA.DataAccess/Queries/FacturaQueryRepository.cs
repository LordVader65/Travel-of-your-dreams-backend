using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class FacturaQueryRepository : IFacturaQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public FacturaQueryRepository(AtraccionesDbContext context) => _context = context;

    public Task<FacturaEntity?> ObtenerPorNumeroAsync(string numero, CancellationToken cancellationToken = default) =>
        _context.Facturas.AsNoTracking().FirstOrDefaultAsync(x => x.fac_numero == numero, cancellationToken);

    public async Task<PagedResult<FacturaEntity>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.Facturas.AsNoTracking()
            .Include(x => x.Reserva).ThenInclude(x => x!.Cliente)
            .Where(x => x.Reserva != null && x.Reserva.Cliente != null && x.Reserva.Cliente.cli_guid == clienteGuid)
            .OrderByDescending(x => x.fac_fecha_emision);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<FacturaEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public async Task<PagedResult<FacturaEntity>> ListarAsync(
        Guid? clienteGuid,
        Guid? reservaGuid,
        string? numero,
        string? estado,
        DateTime? fechaDesdeUtc,
        DateTime? fechaHastaUtc,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Facturas.AsNoTracking()
            .Include(x => x.Reserva).ThenInclude(x => x!.Cliente)
            .Include(x => x.DatosFacturacion)
            .AsQueryable();

        if (clienteGuid.HasValue)
            query = query.Where(x => x.Reserva != null && x.Reserva.Cliente != null && x.Reserva.Cliente.cli_guid == clienteGuid.Value);
        if (reservaGuid.HasValue)
            query = query.Where(x => x.Reserva != null && x.Reserva.rev_guid == reservaGuid.Value);
        if (!string.IsNullOrWhiteSpace(numero))
            query = query.Where(x => x.fac_numero == numero);
        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.fac_estado == estado);
        if (fechaDesdeUtc.HasValue)
            query = query.Where(x => x.fac_fecha_emision >= fechaDesdeUtc.Value);
        if (fechaHastaUtc.HasValue)
            query = query.Where(x => x.fac_fecha_emision <= fechaHastaUtc.Value);

        query = query.OrderByDescending(x => x.fac_fecha_emision);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<FacturaEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }
}
