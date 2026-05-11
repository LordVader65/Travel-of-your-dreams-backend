namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Identity;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<UsuarioResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<UsuarioResponse?> ObtenerPorLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<UsuarioResponse> CrearAsync(CrearUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
    Task<UsuarioResponse> CambiarEstadoAsync(CambiarEstadoUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
    Task<UsuarioResponse> CambiarPasswordAsync(CambiarPasswordRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
    Task<BusinessOperationResult> CambiarRolesAsync(CambiarRolUsuarioRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
}
