using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TravelDreams.Grpc.Atracciones;
using TravelDreams.MsAtracciones.DataAccess.Context;
using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;

namespace TravelDreams.MsAtracciones.Api.Grpc;

public sealed class AtraccionesAvailabilityGrpcService : AtraccionesAvailability.AtraccionesAvailabilityBase
{
    private readonly IAtraccionesPublicService _atracciones;
    private readonly IAvailabilityService _availability;
    private readonly AtraccionesDbContext _db;

    public AtraccionesAvailabilityGrpcService(IAtraccionesPublicService atracciones, IAvailabilityService availability, AtraccionesDbContext db)
    {
        _atracciones = atracciones;
        _availability = availability;
        _db = db;
    }

    public override async Task<GetTicketsResponse> GetTickets(GetTicketsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.AtraccionGuid, out var atraccionGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "AtraccionGuid invalido."));
        }

        var tickets = await _atracciones.ListarTicketsAsync(atraccionGuid, context.CancellationToken);
        var response = new GetTicketsResponse();
        response.Tickets.AddRange(tickets.Select(ticket => new TicketItem
        {
            Guid = ticket.Guid.ToString(),
            Titulo = ticket.Titulo,
            TipoParticipante = ticket.TipoParticipante,
            Precio = ticket.Precio.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Moneda = ticket.Moneda,
            CapacidadMaxima = ticket.CapacidadMaxima
        }));

        return response;
    }

    public override async Task<ReservationContextResponse> GetReservationContext(ReservationContextRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.AtraccionGuid, out var atraccionGuid) ||
            !Guid.TryParse(request.HorarioGuid, out var horarioGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "AtraccionGuid y HorarioGuid son obligatorios."));
        }

        var data = await _db.Horarios
            .AsNoTracking()
            .Where(x => x.hor_guid == horarioGuid
                && x.hor_estado == "A"
                && x.Atraccion != null
                && x.Atraccion.at_guid == atraccionGuid
                && x.Atraccion.at_estado == "A")
            .Select(x => new
            {
                AtraccionGuid = x.Atraccion!.at_guid,
                HorarioGuid = x.hor_guid,
                AtraccionNombre = x.Atraccion.at_nombre,
                Fecha = x.hor_fecha,
                HoraInicio = x.hor_hora_inicio,
                HoraFin = x.hor_hora_fin
            })
            .FirstOrDefaultAsync(context.CancellationToken);

        if (data is null)
        {
            return new ReservationContextResponse { Found = false };
        }

        return new ReservationContextResponse
        {
            Found = true,
            AtraccionGuid = data.AtraccionGuid.ToString(),
            HorarioGuid = data.HorarioGuid.ToString(),
            AtraccionNombre = data.AtraccionNombre,
            HorFecha = data.Fecha.ToString("yyyy-MM-dd"),
            HorHoraInicio = data.HoraInicio.ToString("HH:mm"),
            HorHoraFin = data.HoraFin?.ToString("HH:mm") ?? string.Empty
        };
    }

    public override async Task<AvailabilityResult> ReserveAvailability(TravelDreams.Grpc.Atracciones.ReserveAvailabilityRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.HorarioGuid, out var horarioGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "HorarioGuid invalido."));
        }

        var result = await _availability.ReserveAvailabilityAsync(new Business.DTOs.ReserveAvailabilityRequest
        {
            HorarioGuid = horarioGuid,
            Lines = request.Lines.Select(line => new AvailabilityLineRequest
            {
                TicketGuid = Guid.TryParse(line.TicketGuid, out var ticketGuid) ? ticketGuid : Guid.Empty,
                Cantidad = line.Cantidad
            }).ToList()
        }, context.CancellationToken);

        return Map(result);
    }

    public override async Task<AvailabilityResult> ReleaseAvailability(TravelDreams.Grpc.Atracciones.ReleaseAvailabilityRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.HorarioGuid, out var horarioGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "HorarioGuid invalido."));
        }

        var result = await _availability.ReleaseAvailabilityAsync(new Business.DTOs.ReleaseAvailabilityRequest
        {
            HorarioGuid = horarioGuid,
            Cantidad = request.Cantidad
        }, context.CancellationToken);

        return Map(result);
    }

    private static AvailabilityResult Map(AvailabilityResponse result) => new()
    {
        Success = result.Success,
        Error = result.Error ?? string.Empty,
        AtraccionGuid = result.AtraccionGuid?.ToString() ?? string.Empty,
        HorarioGuid = result.HorarioGuid?.ToString() ?? string.Empty,
        CuposRestantes = result.CuposRestantes
    };
}
