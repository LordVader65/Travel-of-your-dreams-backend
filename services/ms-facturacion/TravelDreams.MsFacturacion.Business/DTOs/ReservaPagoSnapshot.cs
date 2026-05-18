namespace TravelDreams.MsFacturacion.Business.DTOs;

public sealed class ReservaPagoSnapshot
{
    public Guid ReservaGuid { get; set; }
    public Guid ClienteGuid { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaExpiracionUtc { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
}
