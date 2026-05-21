using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin/horarios")]
public sealed class AdminHorariosController : ControllerBase
{
    private readonly IAdminAtraccionesService _admin;

    public AdminHorariosController(IAdminAtraccionesService admin) => _admin = admin;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) => Ok(new { status = 200, data = await _admin.ListarHorariosAsync(ct) });

    [HttpPost]
    public async Task<IActionResult> Crear(AdminHorarioRequest request, CancellationToken ct) =>
        Created(string.Empty, new { status = 201, data = await _admin.GuardarHorarioAsync(null, request, ct) });

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, AdminHorarioRequest request, CancellationToken ct) =>
        Ok(new { status = 200, data = await _admin.GuardarHorarioAsync(guid, request, ct) });

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken ct) =>
        await _admin.DesactivarHorarioAsync(guid, ct) ? NoContent() : NotFound();

    [HttpPost("desactivar-vencidos")]
    public async Task<IActionResult> DesactivarVencidos(CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = new { total = await _admin.DesactivarHorariosVencidosAsync(ct) } });
}
