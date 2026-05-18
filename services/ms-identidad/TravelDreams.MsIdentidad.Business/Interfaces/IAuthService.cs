using TravelDreams.MsIdentidad.Business.DTOs;

namespace TravelDreams.MsIdentidad.Business.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<LoginResponse> LoginAdminAsync(LoginRequest request, CancellationToken ct = default);
    Task<UsuarioResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<UsuarioResponse?> MeAsync(Guid usuarioGuid, CancellationToken ct = default);
}
