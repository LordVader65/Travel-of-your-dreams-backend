namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Auditoria;

public sealed class AuditoriaLogDataModel
{
    public long Id { get; set; }
    public Guid Guid { get; set; }
    public string Tabla { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public int? RegistroId { get; set; }
    public Guid? RegistroGuid { get; set; }
    public string? DatosAnteriores { get; set; }
    public string? DatosNuevos { get; set; }
    public DateTime FechaUtc { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string? OrigenCanal { get; set; }
}
