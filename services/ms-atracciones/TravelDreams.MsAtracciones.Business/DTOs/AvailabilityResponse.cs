namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AvailabilityResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Guid? AtraccionGuid { get; set; }
    public Guid? HorarioGuid { get; set; }
    public int CuposRestantes { get; set; }
}
