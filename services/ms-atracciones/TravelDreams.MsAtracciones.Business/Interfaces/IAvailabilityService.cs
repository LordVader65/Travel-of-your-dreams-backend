using TravelDreams.MsAtracciones.Business.DTOs;

namespace TravelDreams.MsAtracciones.Business.Interfaces;

public interface IAvailabilityService
{
    Task<AvailabilityResponse> ReserveAvailabilityAsync(ReserveAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task<AvailabilityResponse> ReleaseAvailabilityAsync(ReleaseAvailabilityRequest request, CancellationToken cancellationToken = default);
}
