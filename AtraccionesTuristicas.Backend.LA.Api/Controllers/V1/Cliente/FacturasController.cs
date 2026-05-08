using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Cliente;

[Route("api/v{version:apiVersion}/facturas")]
[Authorize]
public sealed class FacturasController : ApiControllerBase
{
    private readonly IFacturaService _facturas;

    public FacturasController(IFacturaService facturas, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _facturas = facturas;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await _facturas.ListarAsync(new FacturaFiltroRequest { ClienteGuid = CurrentUser.ClienteGuid, Page = page, Limit = limit }, CurrentUser, cancellationToken);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken cancellationToken) =>
        OkEnvelope(await _facturas.ObtenerPorGuidAsync(guid, CurrentUser, cancellationToken));
}
