using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;

public interface IDestinoDataService
{
    Task<IReadOnlyList<DestinoDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<DestinoDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DestinoDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<DestinoDataModel> CrearAsync(DestinoDataModel model, CancellationToken cancellationToken = default);
    Task<DestinoDataModel> ActualizarAsync(DestinoDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
