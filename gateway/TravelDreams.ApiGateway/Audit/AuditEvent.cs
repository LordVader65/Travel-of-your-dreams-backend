namespace TravelDreams.ApiGateway.Audit;

public sealed class AuditEvent
{
    public Guid EventoId { get; set; } = Guid.NewGuid();
    public string Servicio { get; set; } = "api-gateway";
    public string Tabla { get; set; } = "http_request";
    public string Operacion { get; set; } = "BUSINESS_EVENT";
    public Guid? RegistroGuid { get; set; }
    public string? DatosAnteriores { get; set; }
    public string? DatosNuevos { get; set; }
    public DateTime? FechaUtc { get; set; } = DateTime.UtcNow;
    public string Usuario { get; set; } = "anonymous";
    public string Ip { get; set; } = "unknown";
    public string? OrigenCanal { get; set; } = "api-gateway";
    public string? CorrelationId { get; set; }
}
