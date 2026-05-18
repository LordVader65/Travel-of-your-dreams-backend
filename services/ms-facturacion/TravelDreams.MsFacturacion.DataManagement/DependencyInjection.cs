using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Services;

namespace TravelDreams.MsFacturacion.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddFacturacionDataManagement(this IServiceCollection services)
    {
        services.AddScoped<IDatosFacturacionDataService, DatosFacturacionDataService>();
        services.AddScoped<IPagoDataService, PagoDataService>();
        services.AddScoped<IFacturacionDataService, FacturacionDataService>();
        return services;
    }
}
