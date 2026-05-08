using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/pagos")]
[Authorize(Policy = "AdminOnly")]
public sealed class PagosAdminController : ApiControllerBase
{
    private readonly IPagoService _pagos;

    public PagosAdminController(IPagoService pagos, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _pagos = pagos;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? reservaGuid, [FromQuery] Guid? clienteGuid, [FromQuery] string? metodo, [FromQuery] string? estado, [FromQuery] DateTime? fechaDesdeUtc, [FromQuery] DateTime? fechaHastaUtc, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _pagos.ListarAsync(new PagoFiltroRequest { ReservaGuid = reservaGuid, ClienteGuid = clienteGuid, Metodo = metodo, Estado = estado, FechaDesdeUtc = fechaDesdeUtc, FechaHastaUtc = fechaHastaUtc, Page = page, Limit = limit }, CurrentUser, ct);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }
}
