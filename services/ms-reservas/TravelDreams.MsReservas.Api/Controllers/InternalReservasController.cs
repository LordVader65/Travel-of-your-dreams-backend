using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("internal/v1/reservas")]
public sealed class InternalReservasController : ControllerBase
{
    private readonly IReservasService _reservas;
    private readonly IClientesService _clientes;

    public InternalReservasController(IReservasService reservas, IClientesService clientes)
    {
        _reservas = reservas;
        _clientes = clientes;
    }

    [HttpGet("/internal/v1/clientes/by-user/{usuarioGuid:guid}")]
    public async Task<IActionResult> ClienteByUsuario(Guid usuarioGuid, CancellationToken ct)
    {
        var cliente = await _clientes.ObtenerPorUsuarioGuidAsync(usuarioGuid, ct);
        return cliente is null
            ? NotFound(new { status = StatusCodes.Status404NotFound, error = "Cliente no encontrado para el usuario." })
            : Ok(new { status = StatusCodes.Status200OK, data = cliente });
    }

    [HttpGet("{guid:guid}/payment-snapshot")]
    public async Task<IActionResult> PaymentSnapshot(Guid guid, CancellationToken ct)
    {
        var reserva = await _reservas.ObtenerAsync(guid, ct);
        if (reserva is null) return NotFound(new { status = StatusCodes.Status404NotFound, error = "Reserva no encontrada." });

        return Ok(new
        {
            status = StatusCodes.Status200OK,
            data = new
            {
                reservaGuid = reserva.Guid,
                revCodigo = reserva.Codigo,
                clienteGuid = reserva.ClienteGuid,
                estado = reserva.Estado,
                fechaExpiracionUtc = reserva.FechaExpiracionUtc,
                subtotal = reserva.Subtotal,
                valorIva = reserva.ValorIva,
                total = reserva.Total,
                moneda = reserva.Moneda
            }
        });
    }

    [HttpPost("{guid:guid}/mark-paid")]
    public async Task<IActionResult> MarkPaid(Guid guid, MarkReservaPaidRequest request, CancellationToken ct)
    {
        if (request.PagoGuid == Guid.Empty || request.FacturaGuid == Guid.Empty)
        {
            return BadRequest(new { status = StatusCodes.Status400BadRequest, error = "PagoGuid y FacturaGuid son obligatorios." });
        }

        var ok = await _reservas.CambiarEstadoAsync(guid, new CambiarEstadoReservaRequest
        {
            Estado = "PAGADA",
            Observacion = $"Pago {request.PagoGuid} / factura {request.FacturaGuid}"
        }, ct);

        return ok ? NoContent() : NotFound(new { status = StatusCodes.Status404NotFound, error = "Reserva no encontrada." });
    }
}

public sealed class MarkReservaPaidRequest
{
    public Guid PagoGuid { get; set; }
    public Guid FacturaGuid { get; set; }
}
