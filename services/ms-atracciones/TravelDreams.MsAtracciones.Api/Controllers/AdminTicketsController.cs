using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin/tickets")]
public sealed class AdminTicketsController : ControllerBase
{
    private readonly IAdminAtraccionesService _admin;

    public AdminTicketsController(IAdminAtraccionesService admin) => _admin = admin;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) => Ok(new { status = 200, data = await _admin.ListarTicketsAsync(ct) });

    [HttpPost]
    public async Task<IActionResult> Crear(AdminTicketRequest request, CancellationToken ct) =>
        Created(string.Empty, new { status = 201, data = await _admin.GuardarTicketAsync(null, request, ct) });

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, AdminTicketRequest request, CancellationToken ct) =>
        Ok(new { status = 200, data = await _admin.GuardarTicketAsync(guid, request, ct) });

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken ct) =>
        await _admin.DesactivarTicketAsync(guid, ct) ? NoContent() : NotFound();
}
