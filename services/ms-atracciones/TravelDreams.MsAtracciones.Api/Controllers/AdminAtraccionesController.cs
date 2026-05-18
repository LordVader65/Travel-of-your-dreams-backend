using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin/atracciones")]
public sealed class AdminAtraccionesController : ControllerBase
{
    private readonly IAdminAtraccionesService _admin;

    public AdminAtraccionesController(IAdminAtraccionesService admin) => _admin = admin;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _admin.ListarAtraccionesAsync(ct) });

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _admin.ObtenerAtraccionAsync(guid, ct);
        return data is null ? NotFound() : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(AdminAtraccionRequest request, CancellationToken ct) =>
        Created(string.Empty, new { status = StatusCodes.Status201Created, data = await _admin.GuardarAtraccionAsync(null, request, ct) });

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, AdminAtraccionRequest request, CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _admin.GuardarAtraccionAsync(guid, request, ct) });

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken ct) =>
        await _admin.DesactivarAtraccionAsync(guid, ct) ? NoContent() : NotFound();
}
