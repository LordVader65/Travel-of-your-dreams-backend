namespace TravelDreams.MsAtracciones.Business.Events.V3;

public sealed class AtraccionesV3Event
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; } = 3;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string Source { get; set; } = "ms-atracciones";
    public AtraccionesEventPayloadV3 Payload { get; set; } = new();
}

public sealed class AtraccionesEventPayloadV3
{
    public string Entidad { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public Guid? Guid { get; set; }
    public int? Id { get; set; }
    public Guid? AtraccionGuid { get; set; }
    public int? AtraccionId { get; set; }
    public Guid? HorarioGuid { get; set; }
    public Guid? TicketGuid { get; set; }
    public string? CatalogoTipo { get; set; }
    public string? Nombre { get; set; }
    public string? Estado { get; set; }
    public int? Cantidad { get; set; }
    public int? CuposRestantes { get; set; }
    public object? Snapshot { get; set; }
}
