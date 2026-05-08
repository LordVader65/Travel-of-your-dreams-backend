using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;

public interface IUsuarioDataService
{
    Task<UsuarioDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<UsuarioDataModel?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<UsuarioDataModel?> ObtenerConRolesAsync(string login, CancellationToken cancellationToken = default);
    Task<UsuarioDataModel> CrearAsync(UsuarioDataModel model, string usuarioRegistro, string ipRegistro, CancellationToken cancellationToken = default);
    Task<UsuarioDataModel?> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken cancellationToken = default);
    Task<UsuarioDataModel?> CambiarPasswordAsync(Guid guid, string passwordHash, string usuario, string ip, CancellationToken cancellationToken = default);
}
