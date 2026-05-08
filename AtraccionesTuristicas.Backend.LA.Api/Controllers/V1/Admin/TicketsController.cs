using AtraccionesTuristicas.Backend.LA.Api.Security;
using AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtraccionesTuristicas.Backend.LA.Api.Controllers.V1.Admin;

[Route("api/v{version:apiVersion}/admin/tickets")]
[Authorize(Policy = "AdminOnly")]
public sealed class TicketsController : ApiControllerBase
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets, ICurrentUserFactory currentUserFactory) : base(currentUserFactory) => _tickets = tickets;

    [HttpGet] public async Task<IActionResult> Listar(CancellationToken ct) => OkEnvelope(await _tickets.ListarAsync(ct));
    [HttpPost] public async Task<IActionResult> Crear(CrearTicketRequest request, CancellationToken ct) { request.UsuarioIngreso = CurrentUser.Login; request.IpIngreso = CurrentUser.Ip; return CreatedEnvelope(await _tickets.CrearAsync(request, CurrentUser, ct)); }
    [HttpPut("{guid:guid}")] public async Task<IActionResult> Actualizar(Guid guid, ActualizarTicketRequest request, CancellationToken ct) { request.Guid = guid; return OkEnvelope(await _tickets.ActualizarAsync(request, CurrentUser, ct)); }
    [HttpDelete("{id:int}")] public async Task<IActionResult> Eliminar(int id, CancellationToken ct) { await _tickets.EliminarAsync(id, CurrentUser, ct); return NoContent(); }
}
