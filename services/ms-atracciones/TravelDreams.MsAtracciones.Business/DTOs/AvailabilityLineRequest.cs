namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AvailabilityLineRequest
{
    public Guid TicketGuid { get; set; }
    public int Cantidad { get; set; }
}
