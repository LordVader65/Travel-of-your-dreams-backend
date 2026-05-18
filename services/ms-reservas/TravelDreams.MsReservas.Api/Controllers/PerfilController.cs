using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("api/v1/me")]
public sealed class PerfilController : ControllerBase
{
    private readonly IClientesService _clientes;

    public PerfilController(IClientesService clientes) => _clientes = clientes;

    [HttpGet]
    public async Task<IActionResult> Obtener([FromQuery] Guid clienteGuid, CancellationToken ct)
    {
        if (clienteGuid == Guid.Empty && TryGetUserGuid(out var usuarioGuid))
        {
            var current = await _clientes.ObtenerPorUsuarioGuidAsync(usuarioGuid, ct);
            clienteGuid = current?.Guid ?? Guid.Empty;
        }

        var data = await _clientes.ObtenerAsync(clienteGuid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Perfil no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromQuery] Guid clienteGuid, ClienteRequest request, CancellationToken ct)
    {
        if (clienteGuid == Guid.Empty && TryGetUserGuid(out var usuarioGuid))
        {
            var current = await _clientes.ObtenerPorUsuarioGuidAsync(usuarioGuid, ct);
            clienteGuid = current?.Guid ?? Guid.Empty;
        }

        request.ClienteGuid = clienteGuid;
        var data = await _clientes.GuardarAsync(request, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    private bool TryGetUserGuid(out Guid usuarioGuid)
    {
        usuarioGuid = Guid.Empty;
        return Request.Headers.TryGetValue("X-User-Guid", out var value)
            && Guid.TryParse(value.ToString(), out usuarioGuid);
    }
}
