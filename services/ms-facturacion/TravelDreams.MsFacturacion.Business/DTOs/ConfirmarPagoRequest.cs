namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class ConfirmarPagoRequest
{
    public Guid? ClienteGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public string Metodo { get; set; } = "TARJETA";
    public decimal Monto { get; set; }
    public string? Moneda { get; set; }
    public string? Referencia { get; set; }
    public string? OrigenCanal { get; set; }
    public string? Observacion { get; set; }
}
