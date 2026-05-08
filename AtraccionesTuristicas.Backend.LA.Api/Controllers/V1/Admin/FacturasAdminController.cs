using AtraccionesTuristicas.Backend.LA.Api.Models.Requests;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/facturas")]
[Authorize(Policy = "AdminOnly")]
public sealed class FacturasAdminController : ApiControllerBase
{
    private readonly IFacturaService _facturas;

    public FacturasAdminController(IFacturaService facturas, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _facturas = facturas;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? clienteGuid, [FromQuery] Guid? reservaGuid, [FromQuery] string? numero, [FromQuery] string? estado, [FromQuery] DateTime? fechaDesdeUtc, [FromQuery] DateTime? fechaHastaUtc, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _facturas.ListarAsync(new FacturaFiltroRequest { ClienteGuid = clienteGuid, ReservaGuid = reservaGuid, Numero = numero, Estado = estado, FechaDesdeUtc = fechaDesdeUtc, FechaHastaUtc = fechaHastaUtc, Page = page, Limit = limit }, CurrentUser, ct);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpPost]
    public async Task<IActionResult> Generar(GenerarFacturaApiRequest request, CancellationToken ct)
    {
        var guid = await _facturas.GenerarAsync(request.ReservaGuid, request.DatosFacturacionGuid, CurrentUser, request.Observacion, request.OrigenCanal, ct);
        return CreatedEnvelope(new { factura_guid = guid });
    }
}
