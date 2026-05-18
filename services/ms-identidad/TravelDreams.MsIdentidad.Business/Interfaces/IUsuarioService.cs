using TravelDreams.MsIdentidad.Business.DTOs;

namespace TravelDreams.MsIdentidad.Business.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken ct = default);
    Task<UsuarioResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default);
    Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CancellationToken ct = default);
    Task<UsuarioResponse?> CambiarEstadoAsync(Guid guid, CambiarEstadoUsuarioRequest request, CancellationToken ct = default);
    Task<UsuarioResponse?> CambiarPasswordAsync(Guid guid, CambiarPasswordRequest request, bool validarActual, CancellationToken ct = default);
    Task<bool> CambiarRolesAsync(Guid guid, CambiarRolesRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<RolResponse>> ListarRolesAsync(CancellationToken ct = default);
}
