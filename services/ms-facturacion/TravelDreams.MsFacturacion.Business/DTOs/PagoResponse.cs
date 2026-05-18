namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class PagoResponse
{
    public Guid Guid { get; set; }
    public Guid ReservaGuid { get; set; }
    public Guid ClienteGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
    public string Metodo { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public DateTime FechaUtc { get; set; }
    public string? OrigenCanal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observacion { get; set; }
}
