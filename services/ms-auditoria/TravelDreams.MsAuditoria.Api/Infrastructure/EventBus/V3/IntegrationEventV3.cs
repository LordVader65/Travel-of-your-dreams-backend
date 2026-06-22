using System.Text.Json;

namespace TravelDreams.MsAuditoria.Api.Infrastructure.EventBus.V3;

public sealed class IntegrationEventV3
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid CorrelationId { get; set; }
    public string Source { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
}
