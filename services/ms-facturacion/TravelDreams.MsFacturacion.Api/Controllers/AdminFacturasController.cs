using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1/admin/facturas")]
public sealed class AdminFacturasController : ControllerBase
{
    private readonly IFacturaService _facturas;

    public AdminFacturasController(IFacturaService facturas) => _facturas = facturas;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? clienteGuid, [FromQuery] Guid? reservaGuid, [FromQuery] string? numero, [FromQuery] string? estado, [FromQuery] DateTime? fechaDesdeUtc, [FromQuery] DateTime? fechaHastaUtc, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var data = await _facturas.ListarAsync(new FacturaFiltroRequest { ClienteGuid = clienteGuid, ReservaGuid = reservaGuid, Numero = numero, Estado = estado, FechaDesdeUtc = fechaDesdeUtc, FechaHastaUtc = fechaHastaUtc, Page = page, Limit = limit }, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _facturas.ObtenerAsync(guid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Factura no encontrada." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Generar(GenerarFacturaRequest request, CancellationToken ct)
    {
        var data = await _facturas.GenerarAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }
}
