using Microsoft.Extensions.DependencyInjection;
using TravelDreams.MsIdentidad.Business.DTOs;
using TravelDreams.MsIdentidad.Business.Interfaces;
using TravelDreams.MsIdentidad.Business.Services;

namespace TravelDreams.MsIdentidad.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentidadBusiness(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var secretKey = configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("JwtSettings:SecretKey debe configurarse con al menos 32 caracteres.");
            }

            return new JwtOptions
            {
                Issuer = configuration["JwtSettings:Issuer"] ?? "TravelDreams",
                Audience = configuration["JwtSettings:Audience"] ?? "TravelDreams.Clients",
                SecretKey = secretKey,
                ExpirationMinutes = int.TryParse(configuration["JwtSettings:ExpirationMinutes"], out var minutes) ? minutes : 120
            };
        });
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        return services;
    }
}
