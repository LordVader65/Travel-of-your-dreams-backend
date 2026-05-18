using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("api/v1/admin/clientes")]
public sealed class AdminClientesController : ControllerBase
{
    private readonly IClientesService _clientes;

    public AdminClientesController(IClientesService clientes) => _clientes = clientes;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? numeroIdentificacion,
        [FromQuery] string? correo,
        [FromQuery] string? estado,
        CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _clientes.ListarAsync(numeroIdentificacion, correo, estado, ct) });

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _clientes.ObtenerAsync(guid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Cliente no encontrado." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(ClienteRequest request, CancellationToken ct)
    {
        var data = await _clientes.GuardarAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, ClienteRequest request, CancellationToken ct)
    {
        request.ClienteGuid = guid;
        var data = await _clientes.GuardarAsync(request, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPut("{guid:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid guid, CambiarEstadoClienteRequest request, CancellationToken ct) =>
        await _clientes.CambiarEstadoAsync(guid, request, ct)
            ? NoContent()
            : NotFound(new { status = StatusCodes.Status404NotFound, error = "Cliente no encontrado." });
}
