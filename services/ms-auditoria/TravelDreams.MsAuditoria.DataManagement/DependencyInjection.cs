using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsAuditoria.DataManagement.Interfaces;
using TravelDreams.MsAuditoria.DataManagement.Services;

namespace TravelDreams.MsAuditoria.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditoriaDataManagement(this IServiceCollection services)
    {
        services.AddScoped<IAuditoriaLogDataService, AuditoriaLogDataService>();
        return services;
    }
}
