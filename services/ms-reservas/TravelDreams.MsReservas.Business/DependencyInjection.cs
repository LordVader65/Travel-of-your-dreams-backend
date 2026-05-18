using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.Client;
using TravelDreams.Grpc.Atracciones;
using TravelDreams.MsReservas.Business.Interfaces;
using TravelDreams.MsReservas.Business.Services;

namespace TravelDreams.MsReservas.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddReservasBusiness(this IServiceCollection services)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        services.AddScoped<IReservasService, ReservasService>();
        services.AddScoped<IClientesService, ClientesService>();
        services.AddScoped(provider =>
        {
            var configuration = provider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var baseUrl = configuration["Services:AtraccionesGrpcUrl"] ?? configuration["Services:AtraccionesUrl"] ?? "http://localhost:5102";
            return new AtraccionesAvailability.AtraccionesAvailabilityClient(GrpcChannel.ForAddress(baseUrl));
        });
        services.AddScoped<IAtraccionesIntegrationClient, AtraccionesGrpcClient>();
        return services;
    }
}
