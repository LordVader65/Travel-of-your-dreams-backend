namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class ReservaDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int HorarioId { get; set; }
    public DateTime FechaReservaUtc { get; set; }
    public DateTime FechaExpiracionUtc { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? OrigenCanal { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public IReadOnlyList<ReservaDetalleDataModel> Detalles { get; set; } = [];
}
