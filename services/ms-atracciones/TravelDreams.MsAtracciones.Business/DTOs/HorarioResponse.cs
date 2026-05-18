namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class HorarioResponse
{
    public Guid Guid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
}
