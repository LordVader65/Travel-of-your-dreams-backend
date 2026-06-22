namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class ConfirmarPagoReceptorRequest
{
    public Guid? CorrelationId { get; set; }
    public string NombreReceptor { get; set; } = string.Empty;
    public string? ApellidoReceptor { get; set; }
    public string CorreoReceptor { get; set; } = string.Empty;
    public string? TelefonoReceptor { get; set; }
    public string? Observacion { get; set; }
    public string? OrigenCanal { get; set; }
}
