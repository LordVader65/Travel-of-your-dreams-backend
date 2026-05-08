using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;

public interface IRolDataService
{
    Task<IReadOnlyList<RolDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<RolDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<RolDataModel?> ObtenerPorDescripcionAsync(string descripcion, CancellationToken cancellationToken = default);
    Task<RolDataModel> CrearAsync(RolDataModel model, CancellationToken cancellationToken = default);
    Task<RolDataModel> ActualizarAsync(RolDataModel model, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
