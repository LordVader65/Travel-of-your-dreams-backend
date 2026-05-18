namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class ReserveAvailabilityRequest
{
    public Guid HorarioGuid { get; set; }
    public IReadOnlyList<AvailabilityLineRequest> Lines { get; set; } = [];
}
