namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class ReleaseAvailabilityRequest
{
    public Guid HorarioGuid { get; set; }
    public int Cantidad { get; set; }
}
