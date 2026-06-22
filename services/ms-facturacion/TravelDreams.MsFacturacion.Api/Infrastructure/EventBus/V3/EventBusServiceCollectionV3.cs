using TravelDreams.MsFacturacion.Business.Events.V3;

namespace TravelDreams.MsFacturacion.Api.Infrastructure.EventBus.V3;

public static class EventBusServiceCollectionV3
{
    public static IServiceCollection AddFacturacionEventBusV3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptionsV3>(configuration.GetSection("RabbitMqV3"));
        services.AddSingleton<IFacturacionEventPublisherV3, RabbitMqFacturacionEventPublisherV3>();
        services.AddHostedService<RabbitMqPagoSolicitudConsumerV3>();
        return services;
    }
}
