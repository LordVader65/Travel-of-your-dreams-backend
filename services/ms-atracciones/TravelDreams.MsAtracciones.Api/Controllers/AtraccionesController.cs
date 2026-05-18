using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/atracciones")]
public sealed class AtraccionesController : ControllerBase
{
    private readonly IAtraccionesPublicService _atracciones;
    private readonly IAdminAtraccionesService _admin;

    public AtraccionesController(IAtraccionesPublicService atracciones, IAdminAtraccionesService admin)
    {
        _atracciones = atracciones;
        _admin = admin;
    }

    [HttpGet("filtros")]
    public async Task<IActionResult> Filtros(CancellationToken cancellationToken)
    {
        var data = new
        {
            destination_filters = await _admin.ListarCatalogoAsync("destinos", cancellationToken),
            type_filters = await _admin.ListarCatalogoAsync("categorias", cancellationToken),
            supported_language_filters = await _admin.ListarCatalogoAsync("idiomas", cancellationToken),
            label_filters = new[] { "free_cancellation", "skip_the_line" },
            time_of_day_filters = new[] { "05:00-12:00", "12:00-18:00", "18:00-05:00" }
        };

        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var data = await _atracciones.ListarAtraccionesAsync(cancellationToken);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken cancellationToken)
    {
        var data = await _atracciones.ObtenerAtraccionAsync(guid, cancellationToken);
        return data is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Atraccion no encontrada." })
            : Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}/tickets")]
    public async Task<IActionResult> Tickets(Guid guid, CancellationToken cancellationToken)
    {
        var data = await _atracciones.ListarTicketsAsync(guid, cancellationToken);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}/horarios-disponibles")]
    public async Task<IActionResult> Horarios(Guid guid, CancellationToken cancellationToken)
    {
        var data = await _atracciones.ListarHorariosPorAtraccionAsync(guid, cancellationToken);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }
}
