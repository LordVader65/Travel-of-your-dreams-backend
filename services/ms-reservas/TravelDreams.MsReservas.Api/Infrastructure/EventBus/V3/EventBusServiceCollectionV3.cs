using TravelDreams.MsReservas.Business.Events.V3;

namespace TravelDreams.MsReservas.Api.Infrastructure.EventBus.V3;

public static class EventBusServiceCollectionV3
{
    public static IServiceCollection AddReservasEventBusV3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptionsV3>(configuration.GetSection("RabbitMqV3"));
        services.AddSingleton<IReservaEventPublisherV3, RabbitMqEventPublisherV3>();
        services.AddHostedService<RabbitMqReservaSolicitudConsumerV3>();
        return services;
    }
}
