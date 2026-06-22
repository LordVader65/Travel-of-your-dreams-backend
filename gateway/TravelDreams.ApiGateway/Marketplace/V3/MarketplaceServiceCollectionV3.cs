namespace TravelDreams.ApiGateway.Marketplace.V3;

public static class MarketplaceServiceCollectionV3
{
    public static IServiceCollection AddMarketplaceV3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptionsV3>(configuration.GetSection("RabbitMqV3"));
        services.AddHttpContextAccessor();
        services.AddHttpClient("marketplace-v3");
        services.AddSingleton<IMarketplaceEventPublisherV3, RabbitMqMarketplaceEventPublisherV3>();
        services.AddScoped<MarketplaceDownstreamClientV3>();
        services.AddGraphQLServer()
            .AddQueryType<MarketplaceQueryV3>()
            .AddMutationType<MarketplaceMutationV3>();
        return services;
    }
}
