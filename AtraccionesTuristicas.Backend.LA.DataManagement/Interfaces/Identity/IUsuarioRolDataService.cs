using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;

public interface IUsuarioRolDataService
{
    Task<IReadOnlyList<UsuarioRolDataModel>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UsuarioRolDataModel>> ListarPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<UsuarioRolDataModel> CrearAsync(UsuarioRolDataModel model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UsuarioRolDataModel>> ReemplazarRolesAsync(int usuarioId, IReadOnlyList<int> rolIds, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
