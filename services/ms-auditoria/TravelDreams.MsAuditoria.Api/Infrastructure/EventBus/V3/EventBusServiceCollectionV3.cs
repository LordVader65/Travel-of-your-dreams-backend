namespace TravelDreams.MsAuditoria.Api.Infrastructure.EventBus.V3;

public static class EventBusServiceCollectionV3
{
    public static IServiceCollection AddAuditoriaEventBusV3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptionsV3>(configuration.GetSection("RabbitMqV3"));
        services.AddHostedService<RabbitMqAuditoriaConsumerV3>();
        return services;
    }
}
