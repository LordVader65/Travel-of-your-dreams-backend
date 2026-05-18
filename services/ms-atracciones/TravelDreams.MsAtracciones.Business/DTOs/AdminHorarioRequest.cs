namespace TravelDreams.MsAtracciones.Business.DTOs;

public sealed class AdminHorarioRequest
{
    public int AtraccionId { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
    public string DiasSemana { get; set; } = "0,1,2,3,4,5,6";
}
