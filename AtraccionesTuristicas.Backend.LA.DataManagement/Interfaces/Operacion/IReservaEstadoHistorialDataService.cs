using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IReservaEstadoHistorialDataService
{
    Task<IReadOnlyList<ReservaEstadoHistorialDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ReservaEstadoHistorialDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
