namespace TravelDreams.MsAtracciones.Business.Events.V3;

public interface IAtraccionesEventPublisherV3
{
    Task PublishAsync(AtraccionesV3Event integrationEvent, string routingKey, CancellationToken ct = default);
}
