namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class GenerarFacturaRequest
{
    public Guid ReservaGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public string? Observacion { get; set; }
    public string? OrigenCanal { get; set; }
}
