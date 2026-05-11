using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;

public sealed class HorarioRepository : RepositoryBase<HorarioEntity>, IHorarioRepository
{
    public HorarioRepository(AtraccionesDbContext context) : base(context) { }

    public Task<HorarioEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.hor_guid == guid, cancellationToken);

    public Task<HorarioEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(x => x.hor_guid == guid, cancellationToken);

    public async Task<HorarioEntity?> MaterializarParaFechaAsync(Guid horarioBaseGuid, DateOnly fecha, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var baseHorario = await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.hor_guid == horarioBaseGuid, cancellationToken);
        if (baseHorario is null) return null;

        var existente = await DbSet.FirstOrDefaultAsync(x =>
            x.at_id == baseHorario.at_id
            && x.hor_fecha == fecha
            && x.hor_hora_inicio == baseHorario.hor_hora_inicio
            && x.hor_hora_fin == baseHorario.hor_hora_fin
            && x.hor_estado == DatabaseConstants.EstadoActivo,
            cancellationToken);

        if (existente is not null) return existente;

        var entity = new HorarioEntity
        {
            hor_guid = Guid.NewGuid(),
            at_id = baseHorario.at_id,
            hor_fecha = fecha,
            hor_hora_inicio = baseHorario.hor_hora_inicio,
            hor_hora_fin = baseHorario.hor_hora_fin,
            hor_cupos_disponibles = baseHorario.hor_cupos_disponibles,
            hor_fecha_ingreso = DateTime.UtcNow,
            hor_usuario_ingreso = usuario,
            hor_ip_ingreso = ip,
            hor_estado = DatabaseConstants.EstadoActivo
        };

        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<HorarioEntity>> ListarDisponiblesPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(x => x.at_id == atraccionId && x.hor_estado == DatabaseConstants.EstadoActivo && x.hor_cupos_disponibles > 0)
            .ToListAsync(cancellationToken);

    public async Task DesactivarHorariosPasadosOSinCuposAsync(string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var horarios = await DbSet
            .Where(x => x.hor_estado == DatabaseConstants.EstadoActivo && (x.hor_fecha < today || x.hor_cupos_disponibles <= 0))
            .ToListAsync(cancellationToken);

        foreach (var horario in horarios)
        {
            horario.hor_estado = DatabaseConstants.EstadoInactivo;
            horario.hor_fecha_mod = DateTime.UtcNow;
            horario.hor_usuario_mod = usuario;
            horario.hor_ip_mod = ip;
        }
    }
}
