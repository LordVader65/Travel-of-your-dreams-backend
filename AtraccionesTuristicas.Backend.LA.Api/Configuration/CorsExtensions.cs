namespace AtraccionesTuristicas.Backend.LA.Api.Configuration;

public static class CorsExtensions
{
    public const string PolicyName = "ApiCors";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (origins.Length == 0)
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(origins);
                }

                policy.AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}
