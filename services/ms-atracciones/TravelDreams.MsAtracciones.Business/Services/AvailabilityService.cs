using TravelDreams.MsAtracciones.Business.DTOs;
using TravelDreams.MsAtracciones.Business.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Models.Availability;

namespace TravelDreams.MsAtracciones.Business.Services;

public sealed class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityDataService _data;

    public AvailabilityService(IAvailabilityDataService data)
    {
        _data = data;
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

        return Map(result);
    }

    public async Task<AvailabilityResponse> ReleaseAvailabilityAsync(ReleaseAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _data.ReleaseAvailabilityAsync(request.HorarioGuid, request.Cantidad, cancellationToken);
        return Map(result);
    }

    private static AvailabilityResponse Map(AvailabilityResultDataModel model) => new()
    {
        Success = model.Success,
        Error = model.Error,
        AtraccionGuid = model.AtraccionGuid,
        HorarioGuid = model.HorarioGuid,
        CuposRestantes = model.CuposRestantes
    };
}
