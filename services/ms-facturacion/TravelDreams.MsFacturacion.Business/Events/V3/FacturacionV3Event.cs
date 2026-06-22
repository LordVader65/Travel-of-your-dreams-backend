namespace TravelDreams.MsFacturacion.Business.Events.V3;

public sealed class FacturacionV3Event
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; } = 3;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string Source { get; set; } = "ms-facturacion";
    public FacturacionEventPayloadV3 Payload { get; set; } = new();
}

public sealed class FacturacionEventPayloadV3
{
    public Guid ReservaGuid { get; set; }
    public string ReservaCodigo { get; set; } = string.Empty;
    public Guid ClienteGuid { get; set; }
    public Guid PagoGuid { get; set; }
    public Guid FacturaGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public string FacturaNumero { get; set; } = string.Empty;
    public string Metodo { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string PagoEstado { get; set; } = string.Empty;
    public string FacturaEstado { get; set; } = string.Empty;
    public string? OrigenCanal { get; set; }
    public string? Observacion { get; set; }
}
