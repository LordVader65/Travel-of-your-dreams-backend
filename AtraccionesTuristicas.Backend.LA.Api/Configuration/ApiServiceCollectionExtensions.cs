using AtraccionesTuristicas.Backend.LA.Api.Filters;
using System.Text.Json;

namespace AtraccionesTuristicas.Backend.LA.Api.Configuration;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddScoped<ValidateModelFilter>();
        services.AddScoped<TrimStringsFilter>();

        services.AddControllers(options =>
        {
            options.Filters.AddService<TrimStringsFilter>();
            options.Filters.AddService<ValidateModelFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        });

        return services;
    }
}
