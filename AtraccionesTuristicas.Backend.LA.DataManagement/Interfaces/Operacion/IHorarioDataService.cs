using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IHorarioDataService
{
    Task<IReadOnlyList<HorarioDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<HorarioDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HorarioDataModel>> ListarDisponiblesPorAtraccionAsync(Guid atraccionGuid, DateOnly? fecha = null, CancellationToken cancellationToken = default);
    Task<HorarioDataModel?> MaterializarParaFechaAsync(Guid horarioBaseGuid, DateOnly fecha, string usuario, string ip, CancellationToken cancellationToken = default);
    Task<HorarioDataModel> CrearAsync(HorarioDataModel model, CancellationToken cancellationToken = default);
    Task<HorarioDataModel?> ActualizarAsync(HorarioDataModel model, CancellationToken cancellationToken = default);
    Task<HorarioDataModel?> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken cancellationToken = default);
    Task<int> DesactivarPasadosOSinCuposAsync(string usuario, string ip, CancellationToken cancellationToken = default);
}
