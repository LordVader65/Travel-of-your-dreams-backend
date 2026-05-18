using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsFacturacion.Business.Interfaces;
using TravelDreams.MsFacturacion.Business.Services;

namespace TravelDreams.MsFacturacion.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddFacturacionBusiness(this IServiceCollection services)
    {
        services.AddScoped<IDatosFacturacionService, DatosFacturacionService>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<IFacturaService, FacturaService>();
        services.AddHttpClient<IReservasIntegrationClient, ReservasHttpClient>((provider, client) =>
        {
            var configuration = provider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var baseUrl = configuration["Services:ReservasUrl"] ?? configuration["Services:ReservasGrpcUrl"] ?? "http://localhost:5103";
            client.BaseAddress = new Uri(baseUrl);
        });
        return services;
    }
}
