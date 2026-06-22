namespace TravelDreams.MsAuditoria.Api.Infrastructure.EventBus.V3;

public sealed class ReservaV3Event
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid CorrelationId { get; set; }
    public string Source { get; set; } = "ms-reservas";
    public ReservaEventPayloadV3 Payload { get; set; } = new();
}

public sealed class ReservaEventPayloadV3
{
    public Guid ReservaGuid { get; set; }
    public string ReservaCodigo { get; set; } = string.Empty;
    public Guid ClienteGuid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public string? AtraccionNombre { get; set; }
    public DateOnly? HorFecha { get; set; }
    public TimeOnly? HorHoraInicio { get; set; }
    public TimeOnly? HorHoraFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? OrigenCanal { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Motivo { get; set; }
    public IReadOnlyList<ReservaLineaEventPayloadV3> Lineas { get; set; } = [];
}

public sealed class ReservaLineaEventPayloadV3
{
    public Guid TicketGuid { get; set; }
    public string TicketTitulo { get; set; } = string.Empty;
    public string TipoParticipante { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
