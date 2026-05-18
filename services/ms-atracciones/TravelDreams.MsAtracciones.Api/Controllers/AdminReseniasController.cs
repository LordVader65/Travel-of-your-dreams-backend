using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin/resenias")]
public sealed class AdminReseniasController : ControllerBase
{
    private readonly IAdminAtraccionesService _admin;

    public AdminReseniasController(IAdminAtraccionesService admin) => _admin = admin;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _admin.ListarReseniasAsync(ct) });

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, [FromQuery] string estado, CancellationToken ct) =>
        await _admin.CambiarEstadoReseniaAsync(guid, estado, ct) ? NoContent() : NotFound();
}
