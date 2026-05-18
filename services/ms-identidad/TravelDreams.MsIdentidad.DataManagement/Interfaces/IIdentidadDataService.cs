using TravelDreams.MsIdentidad.DataManagement.Models;

namespace TravelDreams.MsIdentidad.DataManagement.Interfaces;

public interface IIdentidadDataService
{
    Task<IReadOnlyList<UsuarioDataModel>> ListarUsuariosAsync(CancellationToken ct = default);
    Task<UsuarioDataModel?> ObtenerUsuarioPorGuidAsync(Guid guid, CancellationToken ct = default);
    Task<UsuarioDataModel?> ObtenerUsuarioPorLoginAsync(string login, CancellationToken ct = default);
    Task<UsuarioDataModel> CrearUsuarioAsync(string login, string passwordHash, IEnumerable<int> rolIds, string usuario, string ip, CancellationToken ct = default);
    Task<UsuarioDataModel?> CambiarEstadoUsuarioAsync(Guid guid, string estado, string usuario, string ip, CancellationToken ct = default);
    Task<UsuarioDataModel?> CambiarPasswordAsync(Guid guid, string passwordHash, string usuario, string ip, CancellationToken ct = default);
    Task<bool> ReemplazarRolesAsync(Guid usuarioGuid, IEnumerable<int> rolIds, CancellationToken ct = default);
    Task<IReadOnlyList<RolDataModel>> ListarRolesAsync(CancellationToken ct = default);
    Task<RolDataModel?> ObtenerRolPorDescripcionAsync(string descripcion, CancellationToken ct = default);
}
