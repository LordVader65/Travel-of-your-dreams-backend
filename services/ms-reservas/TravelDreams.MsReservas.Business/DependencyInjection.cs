using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsReservas.Business.Interfaces;
using TravelDreams.MsReservas.Business.Services;

namespace TravelDreams.MsReservas.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddReservasBusiness(this IServiceCollection services)
    {
        services.AddScoped<IReservasService, ReservasService>();
        services.AddScoped<IClientesService, ClientesService>();
        services.AddHttpClient<IAtraccionesIntegrationClient, AtraccionesHttpClient>((provider, client) =>
        {
            var configuration = provider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var baseUrl = configuration["Services:AtraccionesUrl"] ?? configuration["Services:AtraccionesGrpcUrl"] ?? "http://localhost:5102";
            client.BaseAddress = new Uri(baseUrl);
        });
        return services;
    }
}
