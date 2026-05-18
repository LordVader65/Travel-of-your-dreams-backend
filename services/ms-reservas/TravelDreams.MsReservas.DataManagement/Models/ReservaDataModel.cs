namespace TravelDreams.MsReservas.DataManagement.Models;

public sealed class ReservaDataModel
{
    public Guid Guid { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public Guid ClienteGuid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public DateTime FechaReservaUtc { get; set; }
    public DateTime FechaExpiracionUtc { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? OrigenCanal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public IReadOnlyList<ReservaDetalleDataModel> Detalles { get; set; } = [];
}

public sealed class ReservaDetalleDataModel
{
    public Guid TicketGuid { get; set; }
    public string TicketTitulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
