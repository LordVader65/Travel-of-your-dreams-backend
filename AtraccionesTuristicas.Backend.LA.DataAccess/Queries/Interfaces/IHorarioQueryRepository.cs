using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IHorarioQueryRepository
{
    Task<IReadOnlyList<HorarioEntity>> ListarDisponiblesPorAtraccionAsync(Guid atraccionGuid, DateOnly? fecha = null, CancellationToken cancellationToken = default);
}
