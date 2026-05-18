using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("api/v1/reservas")]
public sealed class ReservasController : ControllerBase
{
    private readonly IReservasService _reservas;
    private readonly IClientesService _clientes;

    public ReservasController(IReservasService reservas, IClientesService clientes)
    {
        _reservas = reservas;
        _clientes = clientes;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? clienteGuid, [FromQuery] string? estado, CancellationToken ct)
    {
        clienteGuid ??= await ResolveClienteGuidFromHeadersAsync(ct);
        var data = await _reservas.ListarAsync(clienteGuid, estado, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearReservaRequest request, CancellationToken ct)
    {
        if (!request.ClienteGuid.HasValue && request.ClienteInvitado is null)
        {
            request.ClienteGuid = await ResolveClienteGuidFromHeadersAsync(ct);
        }

        var data = await _reservas.CrearAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _reservas.ObtenerAsync(guid, ct);
        if (data is null) return NotFound(new { status = 404, error = "Reserva no encontrada." });
        if (!await CanAccessReservaAsync(data, ct)) return StatusCode(StatusCodes.Status403Forbidden, new { status = 403, error = "La reserva no pertenece al cliente autenticado." });
        return Ok(new { status = 200, data });
    }

    [HttpPut("{guid:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid guid, CancelarReservaRequest request, CancellationToken ct)
    {
        var reserva = await _reservas.ObtenerAsync(guid, ct);
        if (reserva is null) return NotFound();
        if (!await CanAccessReservaAsync(reserva, ct)) return StatusCode(StatusCodes.Status403Forbidden, new { status = 403, error = "La reserva no pertenece al cliente autenticado." });
        return await _reservas.CancelarAsync(guid, request, ct) ? NoContent() : NotFound();
    }

    private async Task<Guid?> ResolveClienteGuidFromHeadersAsync(CancellationToken ct)
    {
        if (!TryGetUserGuid(out var usuarioGuid)) return null;
        var cliente = await _clientes.ObtenerPorUsuarioGuidAsync(usuarioGuid, ct);
        return cliente?.Guid;
    }

    private async Task<bool> CanAccessReservaAsync(ReservaResponse reserva, CancellationToken ct)
    {
        if (IsAdmin()) return true;
        if (!TryGetUserGuid(out _)) return true;
        var clienteGuid = await ResolveClienteGuidFromHeadersAsync(ct);
        return clienteGuid.HasValue && clienteGuid.Value == reserva.ClienteGuid;
    }

    private bool TryGetUserGuid(out Guid usuarioGuid)
    {
        usuarioGuid = Guid.Empty;
        return Request.Headers.TryGetValue("X-User-Guid", out var value)
            && Guid.TryParse(value.ToString(), out usuarioGuid);
    }

    private bool IsAdmin() =>
        Request.Headers.TryGetValue("X-Roles", out var roles)
        && roles.ToString().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(x => x.Equals("ADMIN", StringComparison.OrdinalIgnoreCase));
}
