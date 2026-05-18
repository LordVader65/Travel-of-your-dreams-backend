using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin/catalogos")]
public sealed class AdminCatalogosController : ControllerBase
{
    private readonly IAdminAtraccionesService _admin;

    public AdminCatalogosController(IAdminAtraccionesService admin) => _admin = admin;

    [HttpGet("{tipo}")]
    public async Task<IActionResult> Listar(string tipo, CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _admin.ListarCatalogoAsync(tipo, ct) });

    [HttpPost("{tipo}")]
    public async Task<IActionResult> Crear(string tipo, AdminCatalogoRequest request, CancellationToken ct) =>
        Created(string.Empty, new { status = StatusCodes.Status201Created, data = await _admin.GuardarCatalogoAsync(tipo, null, request, ct) });

    [HttpPut("{tipo}/{id:int}")]
    public async Task<IActionResult> Actualizar(string tipo, int id, AdminCatalogoRequest request, CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _admin.GuardarCatalogoAsync(tipo, id, request, ct) });

    [HttpDelete("{tipo}/{id:int}")]
    public async Task<IActionResult> Eliminar(string tipo, int id, CancellationToken ct) =>
        await _admin.DesactivarCatalogoAsync(tipo, id, ct) ? NoContent() : NotFound();
}
