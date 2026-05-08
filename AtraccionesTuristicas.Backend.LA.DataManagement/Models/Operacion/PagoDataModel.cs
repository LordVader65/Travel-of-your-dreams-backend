namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class PagoDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int ReservaId { get; set; }
    public string? Referencia { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
    public DateTime FechaUtc { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
}
