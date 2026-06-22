namespace TravelDreams.MsReservas.Business.Events.V3;

public sealed class ReservaV3Event
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; } = 3;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string Source { get; set; } = "ms-reservas";
    public ReservaEventPayloadV3 Payload { get; set; } = new();
}
