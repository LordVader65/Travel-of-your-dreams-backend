using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Events.V3;
using TravelDreams.MsFacturacion.Business.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.Business.Services;

public sealed class PagoService : IPagoService
{
    private readonly IPagoDataService _pagos;
    private readonly IFacturacionDataService _facturacion;
    private readonly IReservasIntegrationClient _reservas;
    private readonly IFacturacionEventPublisherV3 _events;

    public PagoService(
        IPagoDataService pagos,
        IFacturacionDataService facturacion,
        IReservasIntegrationClient reservas,
        IFacturacionEventPublisherV3 events)
    {
        _pagos = pagos;
        _facturacion = facturacion;
        _reservas = reservas;
        _events = events;
    }

    public async Task<PagedResponse<PagoResponse>> ListarAsync(PagoFiltroRequest filtro, CancellationToken ct = default) =>
        FacturacionMappers.Page(await _pagos.ListarAsync(new PagoFiltroDataModel
        {
            ReservaGuid = filtro.ReservaGuid,
            ClienteGuid = filtro.ClienteGuid,
            Metodo = filtro.Metodo,
            Estado = filtro.Estado,
            FechaDesdeUtc = filtro.FechaDesdeUtc,
            FechaHastaUtc = filtro.FechaHastaUtc,
            Page = filtro.Page,
            Limit = filtro.Limit
        }, ct), FacturacionMappers.Pago);

    public async Task<PagoResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var model = await _pagos.ObtenerPorGuidAsync(guid, ct);
        return model is null ? null : FacturacionMappers.Pago(model);
    }

    public async Task<FacturaResponse> ConfirmarPagoYGenerarFacturaAsync(Guid reservaGuid, ConfirmarPagoRequest request, CancellationToken ct = default)
    {
        if (reservaGuid == Guid.Empty) throw new InvalidOperationException("ReservaGuid es obligatorio.");
        if (request.Monto <= 0) throw new InvalidOperationException("Monto debe ser positivo.");

        var reserva = await _reservas.GetPaymentSnapshotAsync(reservaGuid, ct)
            ?? throw new InvalidOperationException("Reserva no encontrada.");

        if (reserva.Estado != "PENDIENTE") throw new InvalidOperationException("La reserva no esta pendiente de pago.");
        if (reserva.FechaExpiracionUtc <= DateTime.UtcNow) throw new InvalidOperationException("La reserva esta expirada.");
        if (request.ClienteGuid.HasValue && request.ClienteGuid.Value != reserva.ClienteGuid) throw new InvalidOperationException("La reserva no pertenece al cliente indicado.");
        if (request.Moneda is not null && request.Moneda != reserva.Moneda) throw new InvalidOperationException("La moneda del pago no coincide con la reserva.");
        if (Math.Round(request.Monto, 2) != Math.Round(reserva.Total, 2)) throw new InvalidOperationException("El monto pagado no coincide con el total de la reserva.");

        var referencia = string.IsNullOrWhiteSpace(request.Referencia)
            ? $"PAY-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Guid.NewGuid():N}"[..32].ToUpperInvariant()
            : request.Referencia.Trim();

        var factura = await _facturacion.RegistrarPagoYFacturaAsync(new CrearPagoDataModel
        {
            ReservaGuid = reserva.ReservaGuid,
            ClienteGuid = reserva.ClienteGuid,
            DatosFacturacionGuid = request.DatosFacturacionGuid,
            Monto = request.Monto,
            Moneda = reserva.Moneda,
            Metodo = string.IsNullOrWhiteSpace(request.Metodo) ? "TARJETA" : request.Metodo.Trim().ToUpperInvariant(),
            Referencia = referencia,
            OrigenCanal = request.OrigenCanal,
            Usuario = "cliente",
            Ip = "api",
            Observacion = request.Observacion
        }, reserva.Subtotal, reserva.ValorIva, ct);

        await _reservas.MarkAsPaidAsync(reserva.ReservaGuid, factura.PagoGuid, factura.Guid, ct);
        await PublishFacturacionEventsAsync(
            reserva,
            factura,
            string.IsNullOrWhiteSpace(request.Metodo) ? "TARJETA" : request.Metodo.Trim().ToUpperInvariant(),
            referencia,
            request.OrigenCanal,
            request.Observacion,
            request.CorrelationId,
            ct);
        return FacturacionMappers.Factura(factura);
    }

    public async Task<FacturaResponse> ConfirmarPagoConReceptorAsync(Guid reservaGuid, ConfirmarPagoReceptorRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.NombreReceptor)) throw new InvalidOperationException("NombreReceptor es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.CorreoReceptor) || !request.CorreoReceptor.Contains('@')) throw new InvalidOperationException("CorreoReceptor es obligatorio y debe ser valido.");

        var reserva = await _reservas.GetPaymentSnapshotAsync(reservaGuid, ct)
            ?? throw new InvalidOperationException("Reserva no encontrada.");

        var observacion = $"Receptor: {request.NombreReceptor.Trim()} {request.ApellidoReceptor?.Trim()} / {request.CorreoReceptor.Trim()}";
        if (!string.IsNullOrWhiteSpace(request.TelefonoReceptor))
        {
            observacion += $" / Tel: {request.TelefonoReceptor.Trim()}";
        }
        if (!string.IsNullOrWhiteSpace(request.Observacion))
        {
            observacion += $" / Obs: {request.Observacion.Trim()}";
        }

        return await ConfirmarPagoYGenerarFacturaAsync(reservaGuid, new ConfirmarPagoRequest
        {
            ClienteGuid = reserva.ClienteGuid,
            Metodo = "BOOKING",
            Monto = reserva.Total,
            Moneda = reserva.Moneda,
            Referencia = $"RECEPTOR-{reservaGuid:N}"[..Math.Min(41, $"RECEPTOR-{reservaGuid:N}".Length)],
            OrigenCanal = string.IsNullOrWhiteSpace(request.OrigenCanal) ? "BOOKING" : request.OrigenCanal,
            Observacion = observacion,
            CorrelationId = request.CorrelationId
        }, ct);
    }

    private async Task PublishFacturacionEventsAsync(
        ReservaPagoSnapshot reserva,
        FacturaDataModel factura,
        string metodo,
        string referencia,
        string? origenCanal,
        string? observacion,
        Guid? requestedCorrelationId,
        CancellationToken ct)
    {
        var correlationId = requestedCorrelationId ?? Guid.NewGuid();
        var payload = new FacturacionEventPayloadV3
        {
            ReservaGuid = reserva.ReservaGuid,
            ReservaCodigo = reserva.Codigo,
            ClienteGuid = reserva.ClienteGuid,
            PagoGuid = factura.PagoGuid,
            FacturaGuid = factura.Guid,
            DatosFacturacionGuid = factura.DatosFacturacionGuid,
            FacturaNumero = factura.Numero,
            Metodo = factura.Pago?.Metodo ?? metodo,
            Referencia = factura.Pago?.Referencia ?? referencia,
            Subtotal = factura.Subtotal,
            ValorIva = factura.ValorIva,
            Total = factura.Total,
            Moneda = factura.Moneda,
            PagoEstado = factura.Pago?.Estado ?? "APROBADO",
            FacturaEstado = factura.Estado,
            OrigenCanal = factura.Pago?.OrigenCanal ?? origenCanal,
            Observacion = factura.Observacion ?? factura.Pago?.Observacion ?? observacion
        };

        await _events.PublishAsync(new FacturacionV3Event
        {
            EventType = "facturacion.v3.pago_confirmado",
            CorrelationId = correlationId,
            Payload = payload
        }, "facturacion.v3.pago_confirmado", ct);

        await _events.PublishAsync(new FacturacionV3Event
        {
            EventType = "facturacion.v3.factura_emitida",
            CorrelationId = correlationId,
            Payload = payload
        }, "facturacion.v3.factura_emitida", ct);
    }
}
