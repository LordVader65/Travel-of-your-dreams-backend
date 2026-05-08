using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IReseniaDataService
{
    Task<IReadOnlyList<ReseniaDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ReseniaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReseniaDataModel> CrearAsync(ReseniaDataModel model, CancellationToken cancellationToken = default);
    Task<ReseniaDataModel> ActualizarAsync(ReseniaDataModel model, CancellationToken cancellationToken = default);
    Task<ReseniaDataModel?> CambiarEstadoAsync(int id, string estado, string usuario, string ip, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
