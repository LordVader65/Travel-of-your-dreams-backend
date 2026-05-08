namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Auditoria;

public sealed class AuditoriaLogFiltroDataModel
{
    public string? Tabla { get; set; }
    public string? Operacion { get; set; }
    public string? Usuario { get; set; }
    public DateTime? DesdeUtc { get; set; }
    public DateTime? HastaUtc { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
