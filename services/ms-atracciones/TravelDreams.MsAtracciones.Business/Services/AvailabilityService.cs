using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Events.V3;
using TravelDreams.MsAtracciones.Business.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Availability;

namespace TravelDreams.MsAtracciones.Business.Services;

public sealed class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityDataService _data;
    private readonly IAtraccionesEventPublisherV3 _events;

    public AvailabilityService(IAvailabilityDataService data, IAtraccionesEventPublisherV3 events)
    {
        _data = data;
        _events = events;
    }

    public async Task<AvailabilityResponse> ReserveAvailabilityAsync(ReserveAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _data.ReserveAvailabilityAsync(
            request.HorarioGuid,
            request.Lines.Select(x => new AvailabilityLineDataModel
            {
                TicketGuid = x.TicketGuid,
                Cantidad = x.Cantidad
            }).ToList(),
            cancellationToken);

        if (result.Success)
        {
            await PublishAvailabilityEventAsync("disponibilidad.v3.reservada", "reservada", result, request.Lines.Sum(x => x.Cantidad), cancellationToken);
        }

        return Map(result);
    }

    public async Task<AvailabilityResponse> ReleaseAvailabilityAsync(ReleaseAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _data.ReleaseAvailabilityAsync(request.HorarioGuid, request.Cantidad, cancellationToken);
        if (result.Success)
        {
            await PublishAvailabilityEventAsync("disponibilidad.v3.liberada", "liberada", result, request.Cantidad, cancellationToken);
        }

        return Map(result);
    }

    private Task PublishAvailabilityEventAsync(string eventType, string accion, AvailabilityResultDataModel result, int cantidad, CancellationToken ct) =>
        _events.PublishAsync(new AtraccionesV3Event
        {
            EventType = eventType,
            Payload = new AtraccionesEventPayloadV3
            {
                Entidad = "disponibilidad",
                Accion = accion,
                Guid = result.HorarioGuid,
                AtraccionGuid = result.AtraccionGuid,
                HorarioGuid = result.HorarioGuid,
                Cantidad = cantidad,
                CuposRestantes = result.CuposRestantes,
                Snapshot = result
            }
        }, eventType, ct);

    private static AvailabilityResponse Map(AvailabilityResultDataModel model) => new()
    {
        Success = model.Success,
        Error = model.Error,
        AtraccionGuid = model.AtraccionGuid,
        HorarioGuid = model.HorarioGuid,
        CuposRestantes = model.CuposRestantes
    };
}
