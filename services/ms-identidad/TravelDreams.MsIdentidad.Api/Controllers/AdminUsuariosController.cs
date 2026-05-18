using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;

namespace TravelDreams.MsIdentidad.Api.Controllers;

[ApiController]
[Route("api/v1/admin/usuarios")]
public sealed class AdminUsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarios;

    public AdminUsuariosController(IUsuarioService usuarios) => _usuarios = usuarios;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _usuarios.ListarAsync(ct) });

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _usuarios.ObtenerAsync(guid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Usuario no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearUsuarioRequest request, CancellationToken ct)
    {
        var data = await _usuarios.CrearAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoUsuarioRequest request, CancellationToken ct)
    {
        var data = await _usuarios.CambiarEstadoAsync(guid, request, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Usuario no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPut("{guid:guid}/password")]
    public async Task<IActionResult> CambiarPassword(Guid guid, CambiarPasswordRequest request, CancellationToken ct)
    {
        var data = await _usuarios.CambiarPasswordAsync(guid, request, validarActual: false, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Usuario no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPut("{guid:guid}/roles")]
    public async Task<IActionResult> CambiarRoles(Guid guid, CambiarRolesRequest request, CancellationToken ct) =>
        await _usuarios.CambiarRolesAsync(guid, request, ct)
            ? NoContent()
            : NotFound(new { status = StatusCodes.Status404NotFound, error = "Usuario no encontrado." });
}
