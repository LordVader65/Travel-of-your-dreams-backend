using AtraccionesTuristicas.Backend.LA.Api.Security;
using System.Security.Claims;

namespace AtraccionesTuristicas.Backend.LA.Api.Configuration;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(context => HasRole(context.User, Roles.Admin)));
            options.AddPolicy("ClienteOnly", policy => policy.RequireAssertion(context => HasRole(context.User, Roles.Cliente)));
        });

        return services;
    }

    private static bool HasRole(ClaimsPrincipal user, string role) =>
        user.FindAll("roles").Concat(user.FindAll(ClaimTypes.Role)).Any(x => string.Equals(x.Value, role, StringComparison.OrdinalIgnoreCase));
}
