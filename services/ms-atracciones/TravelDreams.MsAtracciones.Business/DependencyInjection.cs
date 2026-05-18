using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsAtracciones.Business.Interfaces;
using TravelDreams.MsAtracciones.Business.Services;

namespace TravelDreams.MsAtracciones.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddAtraccionesBusiness(this IServiceCollection services)
    {
        services.AddScoped<IAtraccionesPublicService, AtraccionesPublicService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IAdminAtraccionesService, AdminAtraccionesService>();
        return services;
    }
}
