namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class FacturaDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int ReservaId { get; set; }
    public int? PagoId { get; set; }
    public int? DatosFacturacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observacion { get; set; }
    public string Estado { get; set; } = "A";
}
