using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("api/v1/admin/reservas")]
public sealed class AdminReservasController : ControllerBase
{
    private readonly IReservasService _reservas;

    public AdminReservasController(IReservasService reservas) => _reservas = reservas;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? clienteGuid, [FromQuery] string? estado, CancellationToken ct) =>
        Ok(new { status = 200, data = await _reservas.ListarAsync(clienteGuid, estado, ct) });

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _reservas.ObtenerAsync(guid, ct);
        return data is null ? NotFound() : Ok(new { status = 200, data });
    }

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoReservaRequest request, CancellationToken ct) =>
        await _reservas.CambiarEstadoAsync(guid, request, ct) ? NoContent() : NotFound();

    [HttpPost("expirar-pendientes")]
    public async Task<IActionResult> ExpirarPendientes(CancellationToken ct) =>
        Ok(new { status = 200, total = await _reservas.ExpirarPendientesAsync(ct) });
}
