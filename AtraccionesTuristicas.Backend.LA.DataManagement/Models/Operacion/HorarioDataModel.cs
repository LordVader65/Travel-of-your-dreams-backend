namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class HorarioDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int AtraccionId { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
    public string DiasSemana { get; set; } = "0,1,2,3,4,5,6";
    public string UsuarioIngreso { get; set; } = string.Empty;
    public string IpIngreso { get; set; } = "127.0.0.1";
    public string? UsuarioModificacion { get; set; }
    public string? IpModificacion { get; set; }
    public string Estado { get; set; } = "A";
}
