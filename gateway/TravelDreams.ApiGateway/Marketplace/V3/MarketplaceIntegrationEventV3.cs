namespace TravelDreams.ApiGateway.Marketplace.V3;

public sealed class MarketplaceIntegrationEventV3
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; } = 3;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; }
    public string Source { get; set; } = "api-gateway";
    public object Payload { get; set; } = new();
}

public interface IMarketplaceEventPublisherV3
{
    Task PublishAsync(MarketplaceIntegrationEventV3 integrationEvent, string routingKey, CancellationToken ct = default);
}
