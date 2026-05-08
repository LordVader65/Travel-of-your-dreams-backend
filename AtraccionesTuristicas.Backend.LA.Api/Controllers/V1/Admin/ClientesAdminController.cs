using AtraccionesTuristicas.Backend.LA.Api.Models.Requests;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/clientes")]
[Authorize(Policy = "AdminOnly")]
public sealed class ClientesAdminController : ApiControllerBase
{
    private readonly IClienteService _clientes;
    private readonly IDatosFacturacionService _datosFacturacion;

    public ClientesAdminController(IClienteService clientes, IDatosFacturacionService datosFacturacion, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _clientes = clientes;
        _datosFacturacion = datosFacturacion;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? numeroIdentificacion, [FromQuery] string? correo, [FromQuery] string? estado, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _clientes.ListarAsync(new ClienteFiltroRequest { NumeroIdentificacion = numeroIdentificacion, Correo = correo, Estado = estado, Page = page, Limit = limit }, ct);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct) =>
        OkEnvelope(await _clientes.ObtenerPorGuidAsync(guid, ct));

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoApiRequest request, CancellationToken ct) =>
        OkEnvelope(await _clientes.CambiarEstadoAsync(new CambiarEstadoClienteRequest { ClienteGuid = guid, Estado = request.Estado, Usuario = CurrentUser.Login, Ip = CurrentUser.Ip }, ct));

    [HttpGet("{guid:guid}/datos-facturacion")]
    public async Task<IActionResult> DatosFacturacion(Guid guid, CancellationToken ct) =>
        OkEnvelope(await _datosFacturacion.ListarActivosPorClienteAsync(guid, CurrentUser, ct));
}
