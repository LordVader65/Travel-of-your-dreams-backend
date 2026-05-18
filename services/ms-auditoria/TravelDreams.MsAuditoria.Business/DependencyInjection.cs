using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsAuditoria.Business.Interfaces;
using TravelDreams.MsAuditoria.Business.Services;

namespace TravelDreams.MsAuditoria.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditoriaBusiness(this IServiceCollection services)
    {
        services.AddScoped<IAuditoriaLogService, AuditoriaLogService>();
        return services;
    }
}
