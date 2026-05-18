using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsIdentidad.DataManagement.Interfaces;
using TravelDreams.MsIdentidad.DataManagement.Services;

namespace TravelDreams.MsIdentidad.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentidadDataManagement(this IServiceCollection services)
    {
        services.AddScoped<IIdentidadDataService, IdentidadDataService>();
        return services;
    }
}
