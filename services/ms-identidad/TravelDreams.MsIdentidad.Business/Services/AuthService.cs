using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;
using TravelDreams.MsIdentidad.DataAccess.Common;
using TravelDreams.MsIdentidad.DataManagement.Interfaces;

namespace TravelDreams.MsIdentidad.Business.Services;

public sealed class AuthService : IAuthService
{
    private readonly IIdentidadDataService _data;
    private readonly JwtOptions _jwt;
    private readonly JwtTokenService _tokens;

    public AuthService(IIdentidadDataService data, JwtOptions jwt)
    {
        _data = data;
        _jwt = jwt;
        _tokens = new JwtTokenService(jwt);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        ValidateLogin(request);
        var usuario = await _data.ObtenerUsuarioPorLoginAsync(request.Login, ct)
            ?? throw new InvalidOperationException("Credenciales invalidas.");

        if (usuario.Estado != DatabaseConstants.EstadoActivo) throw new InvalidOperationException("Usuario inactivo.");
        if (!PasswordHasher.Matches(request.Password, usuario.PasswordHash)) throw new InvalidOperationException("Credenciales invalidas.");

        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);
        return new LoginResponse
        {
            UsuarioGuid = usuario.Guid,
            Login = usuario.Login,
            Roles = usuario.Roles.Select(x => x.Descripcion).ToList(),
            Token = _tokens.BuildToken(usuario, expires),
            ExpiraEnUtc = expires
        };
    }

    public async Task<LoginResponse> LoginAdminAsync(LoginRequest request, CancellationToken ct = default)
    {
        var response = await LoginAsync(request, ct);
        if (!response.Roles.Contains(DatabaseConstants.RolAdmin, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El usuario no tiene rol administrador activo.");
        }

        return response;
    }

    public async Task<UsuarioResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        ValidateRegister(request);
        if (await _data.ObtenerUsuarioPorLoginAsync(request.Login, ct) is not null)
        {
            throw new InvalidOperationException("El login ya esta registrado.");
        }

        var rolCliente = await _data.ObtenerRolPorDescripcionAsync(DatabaseConstants.RolCliente, ct)
            ?? throw new InvalidOperationException("Rol CLIENTE no configurado.");

        var usuario = await _data.CrearUsuarioAsync(request.Login, PasswordHasher.Hash(request.Password), [rolCliente.Id], "self-register", "api", ct);
        return IdentidadMappers.Usuario(usuario);
    }

    public async Task<UsuarioResponse?> MeAsync(Guid usuarioGuid, CancellationToken ct = default)
    {
        var usuario = await _data.ObtenerUsuarioPorGuidAsync(usuarioGuid, ct);
        return usuario is null ? null : IdentidadMappers.Usuario(usuario);
    }

    private static void ValidateLogin(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login)) throw new InvalidOperationException("Login es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Password)) throw new InvalidOperationException("Password es obligatorio.");
        request.Login = request.Login.Trim().ToLowerInvariant();
    }

    private static void ValidateRegister(RegisterRequest request)
    {
        ValidateLogin(new LoginRequest { Login = request.Login, Password = request.Password });
        if (!request.Login.Contains('@')) throw new InvalidOperationException("Login debe ser un correo valido.");
        if (request.Password.Length < 8) throw new InvalidOperationException("Password debe tener al menos 8 caracteres.");
        request.Login = request.Login.Trim().ToLowerInvariant();
    }
}
