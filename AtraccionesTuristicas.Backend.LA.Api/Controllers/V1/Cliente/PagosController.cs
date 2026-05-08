using AtraccionesTuristicas.Backend.LA.Api.Models.Requests;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Cliente;

[Route("api/v{version:apiVersion}/pagos")]
[Authorize]
public sealed class PagosController : ApiControllerBase
{
    private readonly IPagoService _pagos;

    public PagosController(IPagoService pagos, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _pagos = pagos;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var result = await _pagos.ListarAsync(new PagoFiltroRequest { ClienteGuid = CurrentUser.ClienteGuid, Page = page, Limit = limit }, CurrentUser, cancellationToken);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar(CrearPagoApiRequest request, CancellationToken cancellationToken)
    {
        var response = await _pagos.ConfirmarPagoYGenerarFacturaAsync(new CrearPagoRequest
        {
            ReservaGuid = request.ReservaGuid,
            Metodo = request.Metodo,
            Monto = request.Monto,
            Referencia = request.Referencia,
            Usuario = CurrentUser.Login,
            Ip = CurrentUser.Ip,
            OrigenCanal = request.OrigenCanal
        }, request.DatosFacturacionGuid, CurrentUser, cancellationToken);

        return CreatedEnvelope(response);
    }
}
