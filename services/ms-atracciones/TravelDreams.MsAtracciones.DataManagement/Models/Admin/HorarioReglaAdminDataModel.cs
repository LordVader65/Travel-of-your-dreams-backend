namespace TravelDreams.MsAtracciones.DataManagement.Models.Admin;

public sealed class HorarioReglaAdminDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int AtraccionId { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public string DiasSemana { get; set; } = "0,1,2,3,4,5,6";
    public int Cupos { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Estado { get; set; } = "A";
}
