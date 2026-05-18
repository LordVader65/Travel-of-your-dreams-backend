using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/resenias")]
public sealed class ReseniasController : ControllerBase
{
    private readonly IAdminAtraccionesService _admin;

    public ReseniasController(IAdminAtraccionesService admin) => _admin = admin;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(new { status = StatusCodes.Status200OK, data = await _admin.ListarReseniasAsync(ct) });

    [HttpPost]
    public async Task<IActionResult> Crear(CrearReseniaRequest request, CancellationToken ct) =>
        Created(string.Empty, new { status = StatusCodes.Status201Created, data = await _admin.CrearReseniaAsync(request, ct) });
}
