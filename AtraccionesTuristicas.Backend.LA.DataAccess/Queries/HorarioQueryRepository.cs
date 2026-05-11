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
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        if (fecha.HasValue)
        {
            if (fecha.Value < today) return [];

            var horarios = await _context.Horarios.AsNoTracking()
                .Include(x => x.Atraccion)
                .Where(x => x.Atraccion != null
                    && x.Atraccion.at_guid == atraccionGuid
                    && x.hor_estado == DatabaseConstants.EstadoActivo
                    && x.hor_cupos_disponibles > 0)
                .ToListAsync(cancellationToken);

            return horarios
                .Where(x => DiaPermitido(x, fecha.Value) && (fecha.Value > today || x.hor_hora_inicio > currentTime))
                .GroupBy(x => new { x.hor_hora_inicio, x.hor_hora_fin })
                .Select(g =>
                {
                    var item = g.OrderByDescending(x => x.hor_fecha == fecha.Value).First();
                    item.hor_fecha = fecha.Value;
                    return item;
                })
                .OrderBy(x => x.hor_hora_inicio)
                .ToList();
        }

        var query = _context.Horarios.AsNoTracking()
            .Include(x => x.Atraccion)
            .Where(x => x.Atraccion != null
                && x.Atraccion.at_guid == atraccionGuid
                && x.hor_estado == DatabaseConstants.EstadoActivo
                && x.hor_cupos_disponibles > 0
                && (x.hor_fecha > today || (x.hor_fecha == today && x.hor_hora_inicio > currentTime)));

        return await query.OrderBy(x => x.hor_fecha).ThenBy(x => x.hor_hora_inicio).ToListAsync(cancellationToken);
    }

    private static bool DiaPermitido(HorarioEntity horario, DateOnly fecha)
    {
        var dias = string.IsNullOrWhiteSpace(horario.hor_dias_semana) ? "0,1,2,3,4,5,6" : horario.hor_dias_semana;
        return dias.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(x => int.TryParse(x, out var dia) && dia == (int)fecha.DayOfWeek);
    }
}
