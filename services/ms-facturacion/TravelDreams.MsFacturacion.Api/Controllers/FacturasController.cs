using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1/facturas")]
public sealed class FacturasController : ControllerBase
{
    private readonly IFacturaService _facturas;
    private readonly IReservasIntegrationClient _reservas;

    public FacturasController(IFacturaService facturas, IReservasIntegrationClient reservas)
    {
        _facturas = facturas;
        _reservas = reservas;
    }

    [HttpGet("mis-facturas")]
    public async Task<IActionResult> MisFacturas([FromQuery] Guid clienteGuid, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        if (clienteGuid == Guid.Empty && TryGetUserGuid(out var usuarioGuid))
        {
            clienteGuid = await _reservas.GetClienteGuidByUsuarioGuidAsync(usuarioGuid, ct) ?? Guid.Empty;
        }

        var data = await _facturas.ListarAsync(new FacturaFiltroRequest { ClienteGuid = clienteGuid, Page = page, Limit = limit }, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _facturas.ObtenerAsync(guid, ct);
        if (data is null) return NotFound(new { status = StatusCodes.Status404NotFound, error = "Factura no encontrada." });
        if (!await CanAccessFacturaAsync(data, ct)) return StatusCode(StatusCodes.Status403Forbidden, new { status = 403, error = "La factura no pertenece al cliente autenticado." });
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    private bool TryGetUserGuid(out Guid usuarioGuid)
    {
        usuarioGuid = Guid.Empty;
        return Request.Headers.TryGetValue("X-User-Guid", out var value)
            && Guid.TryParse(value.ToString(), out usuarioGuid);
    }

    private async Task<bool> CanAccessFacturaAsync(FacturaResponse factura, CancellationToken ct)
    {
        if (IsAdmin()) return true;
        if (!TryGetUserGuid(out var usuarioGuid)) return true;
        var clienteGuid = await _reservas.GetClienteGuidByUsuarioGuidAsync(usuarioGuid, ct);
        return clienteGuid.HasValue && clienteGuid.Value == factura.ClienteGuid;
    }

    private bool IsAdmin() =>
        Request.Headers.TryGetValue("X-Roles", out var roles)
        && roles.ToString().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(x => x.Equals("ADMIN", StringComparison.OrdinalIgnoreCase));
}
