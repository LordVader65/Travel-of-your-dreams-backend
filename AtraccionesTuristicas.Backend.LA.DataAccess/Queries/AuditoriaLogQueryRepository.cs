using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class AuditoriaLogQueryRepository : IAuditoriaLogQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public AuditoriaLogQueryRepository(AtraccionesDbContext context) => _context = context;

    public async Task<PagedResult<AuditoriaLogEntity>> ConsultarAsync(string? tabla, string? operacion, string? usuario, int page, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditoriaLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(tabla))
            query = query.Where(x => x.log_tabla == tabla);
        if (!string.IsNullOrWhiteSpace(operacion))
            query = query.Where(x => x.log_operacion == operacion);
        if (!string.IsNullOrWhiteSpace(usuario))
            query = query.Where(x => x.log_usuario == usuario);

        query = query.OrderByDescending(x => x.log_fecha_utc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<AuditoriaLogEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }
}
