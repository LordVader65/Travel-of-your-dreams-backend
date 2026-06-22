namespace TravelDreams.MsReservas.Business.Events.V3;

public interface IReservaEventPublisherV3
{
    Task PublishAsync(ReservaV3Event integrationEvent, string routingKey, CancellationToken ct = default);
}
