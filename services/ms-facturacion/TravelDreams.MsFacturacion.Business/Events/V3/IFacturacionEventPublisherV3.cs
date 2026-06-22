namespace TravelDreams.MsFacturacion.Business.Events.V3;

public interface IFacturacionEventPublisherV3
{
    Task PublishAsync(FacturacionV3Event integrationEvent, string routingKey, CancellationToken ct = default);
}
