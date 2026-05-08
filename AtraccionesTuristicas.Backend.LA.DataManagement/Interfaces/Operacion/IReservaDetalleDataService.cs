using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IReservaDetalleDataService
{
    Task<IReadOnlyList<ReservaDetalleDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ReservaDetalleDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
