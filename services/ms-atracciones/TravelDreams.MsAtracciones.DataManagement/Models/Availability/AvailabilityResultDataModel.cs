namespace TravelDreams.MsAtracciones.DataManagement.Models.Availability;

public sealed class AvailabilityResultDataModel
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Guid? AtraccionGuid { get; set; }
    public Guid? HorarioGuid { get; set; }
    public int CuposRestantes { get; set; }
}
