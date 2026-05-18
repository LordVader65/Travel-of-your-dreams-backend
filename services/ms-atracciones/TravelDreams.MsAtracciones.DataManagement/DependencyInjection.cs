using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsAtracciones.DataManagement.Interfaces;
using TravelDreams.MsAtracciones.DataManagement.Services;

namespace TravelDreams.MsAtracciones.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddAtraccionesDataManagement(this IServiceCollection services)
    {
        services.AddScoped<IAtraccionesReadDataService, AtraccionesReadDataService>();
        services.AddScoped<IAvailabilityDataService, AvailabilityDataService>();
        services.AddScoped<IAdminAtraccionesDataService, AdminAtraccionesDataService>();
        return services;
    }
}
