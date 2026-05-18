using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("api/v1/booking/reservas")]
public sealed class BookingReservasController : ControllerBase
{
    private const string BookingChannel = "BOOKING";
    private readonly IReservasService _reservas;

    public BookingReservasController(IReservasService reservas)
    {
        _reservas = reservas;
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _reservas.ObtenerAsync(guid, ct);
        if (data is null || !IsBookingReserva(data))
        {
            return NotFound(new { status = StatusCodes.Status404NotFound, error = "Reserva Booking no encontrada." });
        }

        return Ok(new { status = StatusCodes.Status200OK, data });
    }

    [HttpPut("{guid:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid guid, CancelarReservaRequest request, CancellationToken ct)
    {
        var reserva = await _reservas.ObtenerAsync(guid, ct);
        if (reserva is null || !IsBookingReserva(reserva))
        {
            return NotFound(new { status = StatusCodes.Status404NotFound, error = "Reserva Booking no encontrada." });
        }

        return await _reservas.CancelarAsync(guid, request, ct)
            ? NoContent()
            : NotFound(new { status = StatusCodes.Status404NotFound, error = "Reserva Booking no encontrada." });
    }

    private static bool IsBookingReserva(ReservaResponse reserva) =>
        string.Equals(reserva.OrigenCanal, BookingChannel, StringComparison.OrdinalIgnoreCase);
}
