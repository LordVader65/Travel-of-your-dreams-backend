using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class ReservaQueryRepository : IReservaQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public ReservaQueryRepository(AtraccionesDbContext context) => _context = context;

    public Task<ReservaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        _context.Reservas.AsNoTracking()
            .Include(x => x.Detalles).ThenInclude(x => x.Ticket)
            .Include(x => x.Horario).ThenInclude(x => x!.Atraccion)
            .FirstOrDefaultAsync(x => x.rev_guid == guid, cancellationToken);

    public Task<ReservaEntity?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default) =>
        _context.Reservas.AsNoTracking().FirstOrDefaultAsync(x => x.rev_codigo == codigo, cancellationToken);

    public async Task<PagedResult<ReservaEntity>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.Reservas.AsNoTracking()
            .Include(x => x.Cliente)
            .Where(x => x.Cliente != null && x.Cliente.cli_guid == clienteGuid)
            .OrderByDescending(x => x.rev_fecha_reserva_utc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<ReservaEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public async Task<PagedResult<ReservaEntity>> ListarAsync(
        Guid? clienteGuid,
        Guid? atraccionGuid,
        string? codigo,
        string? estado,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Reservas.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Horario).ThenInclude(x => x!.Atraccion)
            .Include(x => x.Detalles).ThenInclude(x => x.Ticket)
            .AsQueryable();

        if (clienteGuid.HasValue)
            query = query.Where(x => x.Cliente != null && x.Cliente.cli_guid == clienteGuid.Value);
        if (atraccionGuid.HasValue)
            query = query.Where(x => x.Horario != null && x.Horario.Atraccion != null && x.Horario.Atraccion.at_guid == atraccionGuid.Value);
        if (!string.IsNullOrWhiteSpace(codigo))
            query = query.Where(x => x.rev_codigo == codigo);
        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.rev_estado == estado);
        if (fechaDesde.HasValue)
            query = query.Where(x => x.Horario != null && x.Horario.hor_fecha >= fechaDesde.Value);
        if (fechaHasta.HasValue)
            query = query.Where(x => x.Horario != null && x.Horario.hor_fecha <= fechaHasta.Value);

        query = query.OrderByDescending(x => x.rev_fecha_reserva_utc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<ReservaEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }
}
