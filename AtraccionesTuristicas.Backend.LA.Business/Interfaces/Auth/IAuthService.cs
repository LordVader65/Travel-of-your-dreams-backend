namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auth
{
    using AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;
public interface IAuthService
{
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<LoginResponse> LoginAdminAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<RegisterClienteResponse> RegistrarClienteAsync(RegisterClienteRequest request, CancellationToken cancellationToken = default);
        Task<BusinessOperationResult> CerrarSesionAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
}

