using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;

namespace TravelDreams.MsIdentidad.Api.Controllers;

[ApiController]
[Route("api/v1/me")]
public sealed class PerfilController : ControllerBase
{
    private readonly IUsuarioService _usuarios;

    public PerfilController(IUsuarioService usuarios) => _usuarios = usuarios;

    [HttpPut("password")]
    public async Task<IActionResult> CambiarPassword([FromQuery] Guid usuarioGuid, CambiarPasswordRequest request, CancellationToken ct)
    {
        var data = await _usuarios.CambiarPasswordAsync(usuarioGuid, request, validarActual: true, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Usuario no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }
}
