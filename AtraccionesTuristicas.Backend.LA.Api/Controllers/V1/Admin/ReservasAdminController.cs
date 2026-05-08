using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Api.Models.Requests;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/reservas")]
[Authorize(Policy = "AdminOnly")]
public sealed class ReservasAdminController : ApiControllerBase
{
    private readonly IReservaService _reservas;

    public ReservasAdminController(IReservaService reservas, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _reservas = reservas;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? clienteGuid, [FromQuery] Guid? atraccionGuid, [FromQuery] string? codigo, [FromQuery] string? estado, [FromQuery] DateOnly? fechaDesde, [FromQuery] DateOnly? fechaHasta, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _reservas.ListarAsync(new ReservaFiltroRequest { ClienteGuid = clienteGuid, AtraccionGuid = atraccionGuid, Codigo = codigo, Estado = estado, FechaDesde = fechaDesde, FechaHasta = fechaHasta, Page = page, Limit = limit }, CurrentUser, ct);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct) => OkEnvelope(await _reservas.ObtenerPorGuidAsync(guid, CurrentUser, ct));

    [HttpPost]
    public async Task<IActionResult> Crear(CrearReservaAdminApiRequest request, CancellationToken ct)
    {
        var guid = await _reservas.CrearAsync(new CrearReservaRequest
        {
            ClienteGuid = request.ClienteGuid,
            HorarioGuid = request.HorGuid,
            Tickets = request.Lineas.Select(x => new CrearReservaDetalleRequest { TicketGuid = x.TckGuid, Cantidad = x.Cantidad }).ToList(),
            Usuario = CurrentUser.Login,
            Ip = CurrentUser.Ip,
            OrigenCanal = request.OrigenCanal,
            ExpiracionMinutos = request.ExpiracionMinutos,
            PorcentajeIva = request.PorcentajeIva
        }, CurrentUser, ct);

        return CreatedEnvelope(await _reservas.ObtenerPorGuidAsync(guid, CurrentUser, ct));
    }

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoReservaAdminRequest request, CancellationToken ct)
    {
        request.ReservaGuid = guid;
        request.Usuario = CurrentUser.Login;
        request.Ip = CurrentUser.Ip;
        await _reservas.CambiarEstadoAdminAsync(request, CurrentUser, ct);
        return NoContent();
    }

    [HttpPost("expirar-pendientes")]
    public async Task<IActionResult> ExpirarPendientes(CancellationToken ct) =>
        OkEnvelope(new { total = await _reservas.ExpirarPendientesAsync(CurrentUser, ct) });
}
