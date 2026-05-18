using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsReservas.DataManagement.Interfaces;
using TravelDreams.MsReservas.DataManagement.Services;

namespace TravelDreams.MsReservas.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddReservasDataManagement(this IServiceCollection services)
    {
        services.AddScoped<IReservasDataService, ReservasDataService>();
        services.AddScoped<IClientesDataService, ClientesDataService>();
        return services;
    }
}
