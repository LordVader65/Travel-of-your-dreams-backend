using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IHorarioRepository : IRepositoryBase<HorarioEntity>
{
    Task<HorarioEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<HorarioEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<HorarioEntity?> MaterializarParaFechaAsync(Guid horarioBaseGuid, DateOnly fecha, string usuario, string ip, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HorarioEntity>> ListarDisponiblesPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default);
    Task DesactivarHorariosPasadosOSinCuposAsync(string usuario, string ip, CancellationToken cancellationToken = default);
}
