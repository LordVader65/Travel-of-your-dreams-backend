using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Auth;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Public;

[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterClienteRequest request, CancellationToken cancellationToken)
    {
        var response = await _auth.RegistrarClienteAsync(request, cancellationToken);
        return CreatedEnvelope(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _auth.LoginAsync(request, cancellationToken);
        return OkEnvelope(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await _auth.CerrarSesionAsync(new LogoutRequest { UsuarioGuid = CurrentUser.UsuarioGuid ?? Guid.Empty }, cancellationToken);
        return OkEnvelope(result);
    }
}
