namespace TravelDreams.MsAtracciones.DataManagement.Models.Public;

public sealed class HorarioPublicoDataModel
{
    public Guid Guid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
}
