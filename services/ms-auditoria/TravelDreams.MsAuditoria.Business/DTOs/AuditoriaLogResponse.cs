namespace TravelDreams.MsAuditoria.Business.DTOs;

public sealed class AuditoriaLogResponse
{
    public long Id { get; set; }
    public Guid Guid { get; set; }
    public string Servicio { get; set; } = string.Empty;
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
    public string? CorrelationId { get; set; }
    public Guid? EventoId { get; set; }
}
