using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Controllers;

[ApiController]
[Route("api/v1/tickets")]
public sealed class TicketsController : ControllerBase
{
    private readonly IAtraccionesPublicService _atracciones;

    public TicketsController(IAtraccionesPublicService atracciones)
    {
        _atracciones = atracciones;
    }

    [HttpGet("{guid:guid}/horarios")]
    public async Task<IActionResult> Horarios(Guid guid, CancellationToken cancellationToken)
    {
        var data = await _atracciones.ListarHorariosPorTicketAsync(guid, cancellationToken);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }
}
