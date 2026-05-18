namespace TravelDreams.MsFacturacion.DataManagement.Models;

public sealed class CrearPagoDataModel
{
    public Guid ReservaGuid { get; set; }
    public Guid ClienteGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
    public string Metodo { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public string? OrigenCanal { get; set; }
    public string Usuario { get; set; } = "cliente";
    public string Ip { get; set; } = "api";
    public string? Observacion { get; set; }
}
