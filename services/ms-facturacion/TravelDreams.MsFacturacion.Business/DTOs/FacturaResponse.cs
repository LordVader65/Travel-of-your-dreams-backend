namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class FacturaResponse
{
    public Guid Guid { get; set; }
    public string Numero { get; set; } = string.Empty;
    public Guid ReservaGuid { get; set; }
    public Guid ClienteGuid { get; set; }
    public Guid PagoGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public DateTime FechaEmisionUtc { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observacion { get; set; }
    public string Estado { get; set; } = "A";
    public PagoResponse? Pago { get; set; }
    public DatosFacturacionResponse? DatosFacturacion { get; set; }
}
