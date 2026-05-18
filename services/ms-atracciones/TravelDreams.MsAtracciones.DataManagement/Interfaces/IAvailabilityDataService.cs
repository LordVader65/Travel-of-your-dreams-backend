using TravelDreams.MsAtracciones.DataManagement.Models.Availability;

namespace TravelDreams.MsAtracciones.DataManagement.Interfaces;

public interface IAvailabilityDataService
{
    Task<AvailabilityResultDataModel> ReserveAvailabilityAsync(Guid horarioGuid, IReadOnlyList<AvailabilityLineDataModel> lines, CancellationToken cancellationToken = default);
    Task<AvailabilityResultDataModel> ReleaseAvailabilityAsync(Guid horarioGuid, int cantidad, CancellationToken cancellationToken = default);
}
