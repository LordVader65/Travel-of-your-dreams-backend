using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1/me/datos-facturacion")]
public sealed class DatosFacturacionController : ControllerBase
{
    private readonly IDatosFacturacionService _datos;

    public DatosFacturacionController(IDatosFacturacionService datos) => _datos = datos;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid clienteGuid, CancellationToken ct)
    {
        var data = await _datos.ListarPorClienteAsync(clienteGuid, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _datos.ObtenerAsync(guid, ct);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Datos de facturacion no encontrados." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(DatosFacturacionRequest request, CancellationToken ct)
    {
        var data = await _datos.GuardarAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> Actualizar(Guid guid, DatosFacturacionRequest request, CancellationToken ct)
    {
        request.Guid = guid;
        var data = await _datos.GuardarAsync(request, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> Eliminar(Guid guid, CancellationToken ct) =>
        await _datos.InactivarAsync(guid, ct)
            ? NoContent()
            : NotFound(new { status = StatusCodes.Status404NotFound, error = "Datos de facturacion no encontrados." });
}
