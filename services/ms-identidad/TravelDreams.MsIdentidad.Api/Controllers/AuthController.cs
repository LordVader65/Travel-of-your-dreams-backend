using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;

namespace TravelDreams.MsIdentidad.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var data = await _auth.RegisterAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var data = await _auth.LoginAsync(request, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost("logout")]
    public IActionResult Logout() =>
        Ok(new { status = StatusCodes.Status200OK, data = new { message = "Sesion cerrada. El token JWT expira segun su vigencia configurada." } });

    [HttpGet("me")]
    public async Task<IActionResult> Me([FromQuery] Guid usuarioGuid, CancellationToken ct)
    {
        var data = await _auth.MeAsync(usuarioGuid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Usuario no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }
}
