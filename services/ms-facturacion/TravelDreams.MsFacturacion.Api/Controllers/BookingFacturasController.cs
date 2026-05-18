using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1/booking/facturas")]
public sealed class BookingFacturasController : ControllerBase
{
    private readonly IFacturaService _facturas;

    public BookingFacturasController(IFacturaService facturas)
    {
        _facturas = facturas;
    }

    [HttpGet]
    public async Task<IActionResult> ListarPorReserva([FromQuery] Guid reservaGuid, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        if (reservaGuid == Guid.Empty)
        {
            return BadRequest(new { status = StatusCodes.Status400BadRequest, error = "reservaGuid es obligatorio." });
        }

        var data = await _facturas.ListarAsync(new FacturaFiltroRequest
        {
            ReservaGuid = reservaGuid,
            Page = page,
            Limit = limit
        }, ct);

        return Ok(new { status = StatusCodes.Status200OK, data });
    }
}
