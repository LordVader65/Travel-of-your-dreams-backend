using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsAtracciones.Business.DTOs;
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

    [HttpGet("{guid:guid}/horarios")]
    public async Task<IActionResult> HorariosContrato(Guid guid, [FromQuery] bool disponibles = true, CancellationToken cancellationToken = default)
    {
        var data = await _atracciones.ListarHorariosPorAtraccionAsync(guid, cancellationToken);
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}/horarios/{horarioGuid:guid}/tickets")]
    public async Task<IActionResult> TicketsPorHorario(Guid guid, Guid horarioGuid, CancellationToken cancellationToken)
    {
        var horarios = await _atracciones.ListarHorariosPorAtraccionAsync(guid, cancellationToken);
        var horario = horarios.FirstOrDefault(x => x.Guid == horarioGuid);
        if (horario is null)
        {
            return NotFound(new { status = StatusCodes.Status404NotFound, error = "Horario no encontrado o sin disponibilidad." });
        }

        var tickets = await _atracciones.ListarTicketsAsync(guid, cancellationToken);
        var data = tickets.Select(ticket => new TicketHorarioResponse
        {
            Guid = ticket.Guid,
            Titulo = ticket.Titulo,
            TipoParticipante = ticket.TipoParticipante,
            Precio = ticket.Precio,
            Moneda = ticket.Moneda,
            CapacidadMaxima = ticket.CapacidadMaxima,
            HorarioGuid = horario.Guid,
            CuposDisponibles = Math.Min(ticket.CapacidadMaxima, horario.CuposDisponibles)
        });

        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpGet("{guid:guid}/resenias")]
    public async Task<IActionResult> Resenias(Guid guid, CancellationToken cancellationToken)
    {
        var data = ToReseniaResponse(await _admin.ListarReseniasPorAtraccionAsync(guid, cancellationToken));
        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPost("{guid:guid}/resenias")]
    public async Task<IActionResult> CrearResenia(Guid guid, CrearReseniaRequest request, CancellationToken cancellationToken)
    {
        request.AtraccionGuid = guid;
        var data = ToReseniaResponse(await _admin.CrearReseniaAsync(request, cancellationToken));
        return Created(string.Empty, new { status = StatusCodes.Status201Created, data });
    }

    private static object ToReseniaResponse(object data)
    {
        if (data is IEnumerable<dynamic> list)
        {
            return list.Select(ToReseniaItem).ToList();
        }

        return ToReseniaItem(data);
    }

    private static object ToReseniaItem(dynamic item) => new
    {
        guid = item.Guid,
        clienteId = item.ClienteId,
        atraccionId = item.AtraccionId,
        atraccionGuid = item.AtraccionGuid,
        reservaGuid = item.ReservaGuid,
        calificacion = item.Calificacion,
        comentario = item.Comentario,
        fecha = item.Fecha,
        estado = item.Estado
    };
}
