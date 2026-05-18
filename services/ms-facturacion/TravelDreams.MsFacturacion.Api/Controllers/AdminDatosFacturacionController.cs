using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v1/admin/clientes/{clienteGuid:guid}/datos-facturacion")]
public sealed class AdminDatosFacturacionController : ControllerBase
{
    private readonly IDatosFacturacionService _datos;

    public AdminDatosFacturacionController(IDatosFacturacionService datos) => _datos = datos;

    [HttpGet]
    public async Task<IActionResult> Listar(Guid clienteGuid, CancellationToken ct)
    {
        var data = await _datos.ListarPorClienteAsync(clienteGuid, ct);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Guid clienteGuid, DatosFacturacionRequest request, CancellationToken ct)
    {
        request.ClienteGuid = clienteGuid;
        var data = await _datos.GuardarAsync(request, ct);
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }
}
