using AtraccionesTuristicas.Backend.LA.Api.Models.Requests;
using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Exceptions;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Cliente;

[Route("api/v{version:apiVersion}/reservas")]
[Authorize]
public sealed class ReservasController : ApiControllerBase
{
    private readonly IReservaService _reservas;
    private readonly IClienteService _clientes;

    public ReservasController(IReservaService reservas, IClienteService clientes, ICurrentUserFactory currentUserFactory) : base(currentUserFactory)
    {
        _reservas = reservas;
        _clientes = clientes;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? estado, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        var result = await _reservas.ListarAsync(new ReservaFiltroRequest { ClienteGuid = clienteGuid, Estado = estado, Page = page, Limit = limit }, CurrentUser, cancellationToken);
        return ListEnvelope(result.Items, result.Page, result.Limit, result.Total);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearReservaApiRequest request, CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        var guid = await _reservas.CrearAsync(new CrearReservaRequest
        {
            ClienteGuid = clienteGuid,
            HorarioGuid = request.HorGuid,
            Fecha = request.Fecha,
            Tickets = request.Lineas.Select(x => new CrearReservaDetalleRequest { TicketGuid = x.TckGuid, Cantidad = x.Cantidad }).ToList(),
            Usuario = CurrentUser.Login,
            Ip = CurrentUser.Ip,
            OrigenCanal = request.OrigenCanal,
            ExpiracionMinutos = request.ExpiracionMinutos,
            PorcentajeIva = request.PorcentajeIva
        }, CurrentUser, cancellationToken);

        var response = await _reservas.ObtenerPorGuidAsync(guid, CurrentUser, cancellationToken);
        return CreatedEnvelope(response);
    }

    [HttpPost("previsualizar")]
    public async Task<IActionResult> Previsualizar(CrearReservaApiRequest request, CancellationToken cancellationToken)
    {
        var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
        var response = await _reservas.PrevisualizarAsync(new CrearReservaRequest
        {
            ClienteGuid = clienteGuid,
            HorarioGuid = request.HorGuid,
            Fecha = request.Fecha,
            Tickets = request.Lineas.Select(x => new CrearReservaDetalleRequest { TicketGuid = x.TckGuid, Cantidad = x.Cantidad }).ToList(),
            Usuario = CurrentUser.Login,
            Ip = CurrentUser.Ip,
            OrigenCanal = request.OrigenCanal,
            ExpiracionMinutos = request.ExpiracionMinutos,
            PorcentajeIva = request.PorcentajeIva
        }, CurrentUser, cancellationToken);

        return OkEnvelope(response);
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken cancellationToken)
    {
        var response = await _reservas.ObtenerPorGuidAsync(guid, CurrentUser, cancellationToken);
        if (!CurrentUser.EsAdmin)
        {
            var clienteGuid = CurrentUser.ClienteGuid ?? throw new ForbiddenBusinessException("El usuario no tiene cliente asociado.");
            var cliente = await _clientes.ObtenerPorGuidAsync(clienteGuid, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
            if (response.ClienteId != cliente.Id) throw new ForbiddenBusinessException("La reserva no pertenece al cliente autenticado.");
        }
        return OkEnvelope(response);
    }

    [HttpPost("{guid:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid guid, [FromBody] CancelarReservaRequest request, CancellationToken cancellationToken)
    {
        request.ReservaGuid = guid;
        request.Usuario = CurrentUser.Login;
        request.Ip = CurrentUser.Ip;
        return OkEnvelope(await _reservas.CancelarAsync(request, CurrentUser, cancellationToken));
    }
}
