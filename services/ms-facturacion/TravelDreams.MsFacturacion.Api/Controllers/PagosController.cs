using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class PagosController : ControllerBase
{
    private readonly IPagoService _pagos;

    public PagosController(IPagoService pagos) => _pagos = pagos;

    [HttpPost("reservas/{guid:guid}/confirmar-pago")]
    public async Task<IActionResult> ConfirmarPago(Guid guid, ConfirmarPagoRequest request, CancellationToken ct)
    {
        var data = await _pagos.ConfirmarPagoYGenerarFacturaAsync(guid, request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpPost("reservas/{guid:guid}/confirmar-pago-receptor")]
    public async Task<IActionResult> ConfirmarPagoReceptor(Guid guid, ConfirmarPagoReceptorRequest request, CancellationToken ct)
    {
        var data = await _pagos.ConfirmarPagoConReceptorAsync(guid, request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpGet("pagos")]
    public async Task<IActionResult> MisPagos([FromServices] IReservasIntegrationClient reservas, [FromQuery] Guid clienteGuid, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        if (clienteGuid == Guid.Empty && TryGetUserGuid(out var usuarioGuid))
        {
            clienteGuid = await reservas.GetClienteGuidByUsuarioGuidAsync(usuarioGuid, ct) ?? Guid.Empty;
        }

        var data = await _pagos.ListarAsync(new PagoFiltroRequest { ClienteGuid = clienteGuid, Page = page, Limit = limit }, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    private bool TryGetUserGuid(out Guid usuarioGuid)
    {
        usuarioGuid = Guid.Empty;
        return Request.Headers.TryGetValue("X-User-Guid", out var value)
            && Guid.TryParse(value.ToString(), out usuarioGuid);
    }
}
