using Grpc.Core;
using TravelDreams.Grpc.Atracciones;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Business.Services;

public sealed class AtraccionesGrpcClient : IAtraccionesIntegrationClient
{
    private readonly AtraccionesAvailability.AtraccionesAvailabilityClient _client;

    public AtraccionesGrpcClient(AtraccionesAvailability.AtraccionesAvailabilityClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<AtraccionTicketDto>> GetTicketsAsync(Guid atraccionGuid, CancellationToken ct = default)
    {
        var response = await _client.GetTicketsAsync(new GetTicketsRequest
        {
            AtraccionGuid = atraccionGuid.ToString()
        }, cancellationToken: ct);

        return response.Tickets.Select(ticket => new AtraccionTicketDto
        {
            Guid = Guid.Parse(ticket.Guid),
            Titulo = ticket.Titulo,
            TipoParticipante = ticket.TipoParticipante,
            Precio = decimal.Parse(ticket.Precio, System.Globalization.CultureInfo.InvariantCulture),
            Moneda = ticket.Moneda,
            CapacidadMaxima = ticket.CapacidadMaxima
        }).ToList();
    }

    public async Task ReserveAsync(Guid horarioGuid, IReadOnlyList<CrearReservaLineaRequest> lineas, CancellationToken ct = default)
    {
        var request = new TravelDreams.Grpc.Atracciones.ReserveAvailabilityRequest
        {
            HorarioGuid = horarioGuid.ToString()
        };
        request.Lines.AddRange(lineas.Select(line => new AvailabilityLine
        {
            TicketGuid = line.TicketGuid.ToString(),
            Cantidad = line.Cantidad
        }));

        var response = await _client.ReserveAvailabilityAsync(request, cancellationToken: ct);
        EnsureSuccess(response);
    }

    public async Task ReleaseAsync(Guid horarioGuid, int cantidad, CancellationToken ct = default)
    {
        var response = await _client.ReleaseAvailabilityAsync(new ReleaseAvailabilityRequest
        {
            HorarioGuid = horarioGuid.ToString(),
            Cantidad = cantidad
        }, cancellationToken: ct);

        EnsureSuccess(response);
    }

    private static void EnsureSuccess(AvailabilityResult response)
    {
        if (!response.Success)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, response.Error));
        }
    }
}
