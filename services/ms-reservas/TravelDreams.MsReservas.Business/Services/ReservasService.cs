using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Events.V3;
using TravelDreams.MsReservas.Business.Interfaces;
using TravelDreams.MsReservas.DataManagement.Interfaces;
using TravelDreams.MsReservas.DataManagement.Models;

namespace TravelDreams.MsReservas.Business.Services;

public sealed class ReservasService : IReservasService
{
    private readonly IReservasDataService _data;
    private readonly IAtraccionesIntegrationClient _atracciones;
    private readonly IReservaEventPublisherV3 _eventsV3;

    public ReservasService(IReservasDataService data, IAtraccionesIntegrationClient atracciones, IReservaEventPublisherV3 eventsV3)
    {
        _data = data;
        _atracciones = atracciones;
        _eventsV3 = eventsV3;
    }

    public async Task<IReadOnlyList<ReservaResponse>> ListarAsync(Guid? clienteGuid, string? estado, CancellationToken ct = default)
    {
        await ExpirarPendientesAsync(ct);
        return (await _data.ListarAsync(clienteGuid, estado, ct)).Select(Map).ToList();
    }

    public async Task<IReadOnlyList<ReservaResponse>> ListarPorCanalAsync(string origenCanal, string? estado, CancellationToken ct = default)
    {
        await ExpirarPendientesAsync(ct);
        return (await _data.ListarPorCanalAsync(origenCanal, estado, ct)).Select(Map).ToList();
    }

    public async Task<ReservaResponse?> ObtenerAsync(Guid reservaGuid, CancellationToken ct = default)
    {
        await ExpirarPendientesAsync(ct);
        var reserva = await _data.ObtenerAsync(reservaGuid, ct);
        return reserva is null ? null : Map(reserva);
    }

    public async Task<ReservaResponse> CrearAsync(CrearReservaRequest request, CancellationToken ct = default)
    {
        if (request.AtraccionGuid == Guid.Empty || request.HorarioGuid == Guid.Empty || request.Lineas.Count == 0)
        {
            throw new InvalidOperationException("AtraccionGuid, HorarioGuid y lineas son obligatorios.");
        }

        var clienteGuid = request.ClienteGuid;
        if (!clienteGuid.HasValue || clienteGuid == Guid.Empty)
        {
            if (request.ClienteInvitado is null) throw new InvalidOperationException("Debe enviar ClienteGuid o cliente_invitado.");
            clienteGuid = (await _data.UpsertClienteAsync(new ClienteDataModel
            {
                Guid = request.ClienteInvitado.ClienteGuid,
                UsuarioGuid = request.ClienteInvitado.UsuarioGuid,
                TipoIdentificacion = request.ClienteInvitado.TipoIdentificacion,
                NumeroIdentificacion = request.ClienteInvitado.NumeroIdentificacion,
                Nombres = request.ClienteInvitado.Nombres,
                Apellidos = request.ClienteInvitado.Apellidos,
                RazonSocial = request.ClienteInvitado.RazonSocial,
                Correo = request.ClienteInvitado.Correo,
                Telefono = request.ClienteInvitado.Telefono,
                Direccion = request.ClienteInvitado.Direccion
            }, ct)).Guid;
        }

        var context = await _atracciones.GetReservationContextAsync(request.AtraccionGuid, request.HorarioGuid, ct);
        var tickets = await _atracciones.GetTicketsAsync(request.AtraccionGuid, ct);
        var lineas = new List<CrearReservaLineaDataModel>();
        foreach (var linea in request.Lineas)
        {
            if (linea.Cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser positiva.");
            var ticket = tickets.FirstOrDefault(x => x.Guid == linea.TicketGuid)
                ?? throw new InvalidOperationException("Ticket no encontrado para la atraccion.");
            if (linea.Cantidad > ticket.CapacidadMaxima) throw new InvalidOperationException("La cantidad supera la capacidad maxima del ticket.");
            lineas.Add(new CrearReservaLineaDataModel { TicketGuid = ticket.Guid, TicketTitulo = ticket.Titulo, TipoParticipante = ticket.TipoParticipante, Cantidad = linea.Cantidad, PrecioUnitario = ticket.Precio });
        }

        var reserved = false;
        try
        {
            await _atracciones.ReserveAsync(request.HorarioGuid, request.Lineas, ct);
            reserved = true;
            var guid = await _data.CrearAsync(new CrearReservaDataModel
            {
                ClienteGuid = clienteGuid!.Value,
                AtraccionGuid = request.AtraccionGuid,
                HorarioGuid = request.HorarioGuid,
                AtraccionNombre = context.AtraccionNombre,
                HorFecha = context.HorFecha,
                HorHoraInicio = context.HorHoraInicio,
                HorHoraFin = context.HorHoraFin,
                Lineas = lineas,
                OrigenCanal = request.OrigenCanal,
                ExpiracionMinutos = request.ExpiracionMinutos,
                PorcentajeIva = request.PorcentajeIva,
                Usuario = "cliente",
                Ip = "api"
            }, ct);
            var reserva = await _data.ObtenerAsync(guid, ct) ?? throw new InvalidOperationException("Reserva no encontrada despues de crear.");
            await PublishReservaEventV3Async(reserva, "reserva.v3.creada", "reserva.v3.creada", null, ct, request.CorrelationId);
            return Map(reserva);
        }
        catch
        {
            if (reserved)
            {
                await _atracciones.ReleaseAsync(request.HorarioGuid, request.Lineas.Sum(x => x.Cantidad), CancellationToken.None);
            }
            throw;
        }
    }

    public async Task<bool> CancelarAsync(Guid reservaGuid, CancelarReservaRequest request, CancellationToken ct = default)
    {
        var reserva = await _data.ObtenerAsync(reservaGuid, ct) ?? throw new InvalidOperationException("Reserva no encontrada.");
        var ok = await _data.CancelarAsync(reservaGuid, request.Motivo, "cliente", "api", ct);
        if (ok)
        {
            await _atracciones.ReleaseAsync(reserva.HorarioGuid, reserva.Detalles.Sum(x => x.Cantidad), ct);
            var cancelada = await _data.ObtenerAsync(reservaGuid, ct) ?? reserva;
            await PublishReservaEventV3Async(cancelada, "reserva.v3.cancelada", "reserva.v3.cancelada", request.Motivo, ct);
        }
        return ok;
    }

    public async Task<int> ExpirarPendientesAsync(CancellationToken ct = default)
    {
        var expiradas = await _data.ExpirarPendientesAsync("system", "worker", (horarioGuid, cantidad, token) => _atracciones.ReleaseAsync(horarioGuid, cantidad, token), ct);
        foreach (var reserva in expiradas)
        {
            var isBooking = string.Equals(reserva.OrigenCanal, "BOOKING", StringComparison.OrdinalIgnoreCase);
            var routingKey = isBooking ? "reserva.v3.cancelada" : "reserva.v3.expirada";
            var eventType = routingKey;
            var motivo = isBooking ? "EXPIRACION_AUTOMATICA_BOOKING" : "EXPIRACION_AUTOMATICA";
            await PublishReservaEventV3Async(reserva, eventType, routingKey, motivo, ct);
        }

        return expiradas.Count;
    }

    public async Task<bool> CambiarEstadoAsync(Guid reservaGuid, CambiarEstadoReservaRequest request, CancellationToken ct = default)
    {
        var ok = await _data.CambiarEstadoAsync(reservaGuid, request.Estado, "admin", "api", request.Observacion, ct);
        if (!ok) return false;

        var reserva = await _data.ObtenerAsync(reservaGuid, ct);
        if (reserva is null) return true;

        if (string.Equals(request.Estado, "PAGADA", StringComparison.OrdinalIgnoreCase))
        {
            await PublishReservaEventV3Async(reserva, "reserva.v3.pagada", "reserva.v3.pagada", request.Observacion, ct);
        }
        else if (string.Equals(request.Estado, "CANCELADA", StringComparison.OrdinalIgnoreCase))
        {
            await PublishReservaEventV3Async(reserva, "reserva.v3.cancelada", "reserva.v3.cancelada", request.Observacion, ct);
        }

        return true;
    }

    private Task PublishReservaEventV3Async(
        ReservaDataModel reserva,
        string eventType,
        string routingKey,
        string? motivo,
        CancellationToken ct,
        Guid? correlationId = null)
    {
        var payload = new ReservaEventPayloadV3
        {
            ReservaGuid = reserva.Guid,
            ReservaCodigo = reserva.Codigo,
            ClienteGuid = reserva.ClienteGuid,
            AtraccionGuid = reserva.AtraccionGuid,
            HorarioGuid = reserva.HorarioGuid,
            AtraccionNombre = reserva.AtraccionNombre,
            HorFecha = reserva.HorFecha,
            HorHoraInicio = reserva.HorHoraInicio,
            HorHoraFin = reserva.HorHoraFin,
            Estado = reserva.Estado,
            OrigenCanal = reserva.OrigenCanal,
            Subtotal = reserva.Subtotal,
            ValorIva = reserva.ValorIva,
            Total = reserva.Total,
            Moneda = reserva.Moneda,
            Motivo = motivo,
            Lineas = reserva.Detalles.Select(x => new ReservaLineaEventPayloadV3
            {
                TicketGuid = x.TicketGuid,
                TicketTitulo = x.TicketTitulo,
                TipoParticipante = x.TipoParticipante,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario,
                Subtotal = x.Subtotal
            }).ToList()
        };

        return _eventsV3.PublishAsync(new ReservaV3Event
        {
            EventType = eventType,
            CorrelationId = correlationId ?? Guid.NewGuid(),
            Payload = payload
        }, routingKey, ct);
    }

    private static ReservaResponse Map(ReservaDataModel model) => new()
    {
        Guid = model.Guid,
        Codigo = model.Codigo,
        ClienteGuid = model.ClienteGuid,
        AtraccionGuid = model.AtraccionGuid,
        HorarioGuid = model.HorarioGuid,
        AtraccionNombre = model.AtraccionNombre,
        HorFecha = model.HorFecha,
        HorHoraInicio = model.HorHoraInicio,
        HorHoraFin = model.HorHoraFin,
        FechaReservaUtc = model.FechaReservaUtc,
        FechaExpiracionUtc = model.FechaExpiracionUtc,
        Subtotal = model.Subtotal,
        ValorIva = model.ValorIva,
        Total = model.Total,
        Moneda = model.Moneda,
        OrigenCanal = model.OrigenCanal,
        Estado = model.Estado,
        Detalles = model.Detalles.Select(x => new ReservaDetalleResponse { TicketGuid = x.TicketGuid, TicketTitulo = x.TicketTitulo, TipoParticipante = x.TipoParticipante, Cantidad = x.Cantidad, PrecioUnitario = x.PrecioUnitario, Subtotal = x.Subtotal }).ToList()
    };
}
