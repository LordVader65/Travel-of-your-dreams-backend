using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class HorarioQueryRepository : IHorarioQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public HorarioQueryRepository(AtraccionesDbContext context) => _context = context;

    public async Task<IReadOnlyList<HorarioEntity>> ListarDisponiblesPorAtraccionAsync(Guid atraccionGuid, DateOnly? fecha = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Horarios.AsNoTracking()
            .Include(x => x.Atraccion)
            .Where(x => x.Atraccion != null
                && x.Atraccion.at_guid == atraccionGuid
                && x.hor_estado == DatabaseConstants.EstadoActivo
                && x.hor_cupos_disponibles > 0);

        if (fecha.HasValue)
            query = query.Where(x => x.hor_fecha == fecha.Value);

        return await query.OrderBy(x => x.hor_fecha).ThenBy(x => x.hor_hora_inicio).ToListAsync(cancellationToken);
    }
}
