using TravelDreams.MsAtracciones.Business.Events.V3;

namespace TravelDreams.MsAtracciones.Api.Infrastructure.EventBus.V3;

public static class EventBusServiceCollectionV3
{
    public static IServiceCollection AddAtraccionesEventBusV3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptionsV3>(configuration.GetSection("RabbitMqV3"));
        services.AddSingleton<IAtraccionesEventPublisherV3, RabbitMqAtraccionesEventPublisherV3>();
        return services;
    }
}
