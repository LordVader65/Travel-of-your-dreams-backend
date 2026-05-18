using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1/admin/pagos")]
public sealed class AdminPagosController : ControllerBase
{
    private readonly IPagoService _pagos;

    public AdminPagosController(IPagoService pagos) => _pagos = pagos;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? reservaGuid, [FromQuery] Guid? clienteGuid, [FromQuery] string? metodo, [FromQuery] string? estado, [FromQuery] DateTime? fechaDesdeUtc, [FromQuery] DateTime? fechaHastaUtc, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var data = await _pagos.ListarAsync(new PagoFiltroRequest { ReservaGuid = reservaGuid, ClienteGuid = clienteGuid, Metodo = metodo, Estado = estado, FechaDesdeUtc = fechaDesdeUtc, FechaHastaUtc = fechaHastaUtc, Page = page, Limit = limit }, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _pagos.ObtenerAsync(guid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Pago no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }
}
