using System.Text;
using AtraccionesTuristicas.Backend.LA.Api.Models.Common;
using AtraccionesTuristicas.Backend.LA.Business.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AtraccionesTuristicas.Backend.LA.Api.Configuration;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtOptions options)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(config =>
            {
                config.MapInboundClaims = false;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };

                config.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(ApiErrorResponse.Create(
                            StatusCodes.Status401Unauthorized,
                            "Token ausente o invalido.",
                            context.Request.Path));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(ApiErrorResponse.Create(
                            StatusCodes.Status403Forbidden,
                            "No tiene permisos para ejecutar esta operacion.",
                            context.Request.Path));
                    }
                };
            });

        return services;
    }
}
