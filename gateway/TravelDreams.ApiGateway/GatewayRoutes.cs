namespace TravelDreams.ApiGateway;

public static class GatewayRoutes
{
    public static IReadOnlyList<GatewayRoute> All { get; } =
    [
        new()
        {
            Name = "v2-facturacion-reservas",
            ServiceKey = "FacturacionUrl",
            Methods = ["POST"],
            Prefixes = ["/api/v2/reservas/"],
            Contains = ["/pagos/confirmacion"]
        },
        new()
        {
            Name = "v2-reservas-post",
            ServiceKey = "ReservasUrl",
            Methods = ["POST"],
            Prefixes = ["/api/v2/reservas"],
            ValidateTokenWhenPresent = true,
            AllowedRoles = ["CLIENTE", "ADMIN", "BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "v2-reservas",
            ServiceKey = "ReservasUrl",
            Methods = ["GET"],
            Prefixes = ["/api/v2/reservas"],
            RequiresAuthentication = true,
            AllowedRoles = ["CLIENTE", "ADMIN", "BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "v2-atracciones",
            ServiceKey = "AtraccionesUrl",
            Methods = ["GET"],
            Prefixes = ["/api/v2/atracciones"]
        },
        new()
        {
            Name = "facturacion-reservas",
            ServiceKey = "FacturacionUrl",
            Methods = ["POST"],
            Prefixes = ["/api/v1/reservas/"],
            Contains = ["/confirmar-pago", "/pagos/confirmacion"],
            ValidateTokenWhenPresent = true,
            AllowedRoles = ["CLIENTE", "ADMIN", "BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "atracciones-resenias-write",
            ServiceKey = "AtraccionesUrl",
            Methods = ["POST"],
            Prefixes =
            [
                "/api/v1/atracciones/"
            ],
            Contains = ["/resenias"],
            ValidateTokenWhenPresent = true,
            AllowedRoles = ["CLIENTE", "ADMIN", "BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "resenias-write",
            ServiceKey = "AtraccionesUrl",
            Methods = ["POST"],
            Prefixes =
            [
                "/api/v1/resenias"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["CLIENTE", "ADMIN"]
        },
        new()
        {
            Name = "booking-reservas",
            ServiceKey = "ReservasUrl",
            Prefixes =
            [
                "/api/v1/booking/reservas"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "booking-facturas",
            ServiceKey = "FacturacionUrl",
            Prefixes =
            [
                "/api/v1/booking/facturas"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "facturacion-admin-datos",
            ServiceKey = "FacturacionUrl",
            Methods = ["GET", "POST"],
            Prefixes = ["/api/v1/admin/clientes/"],
            Contains = ["/datos-facturacion"],
            RequiresAuthentication = true,
            AllowedRoles = ["ADMIN"]
        },
        new()
        {
            Name = "identidad-auth-me",
            ServiceKey = "IdentidadUrl",
            Methods = ["GET", "POST"],
            Prefixes =
            [
                "/api/v1/auth/me",
                "/api/v1/auth/logout"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["CLIENTE", "ADMIN"]
        },
        new()
        {
            Name = "identidad-admin",
            ServiceKey = "IdentidadUrl",
            Prefixes =
            [
                "/api/v1/admin/usuarios",
                "/api/v1/admin/roles"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["ADMIN"]
        },
        new()
        {
            Name = "identidad-perfil",
            ServiceKey = "IdentidadUrl",
            Prefixes =
            [
                "/api/v1/me/password"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["CLIENTE", "ADMIN"]
        },
        new()
        {
            Name = "identidad",
            ServiceKey = "IdentidadUrl",
            Prefixes =
            [
                "/api/v1/auth",
                "/api/v1/admin/auth",
                "/api/v1/me/password"
            ],
            ValidateTokenWhenPresent = true
        },
        new()
        {
            Name = "atracciones",
            ServiceKey = "AtraccionesUrl",
            Prefixes =
            [
                "/api/v1/atracciones",
                "/api/v1/tickets",
                "/api/v1/resenias",
                "/api/v1/admin/atracciones",
                "/api/v1/admin/catalogos",
                "/api/v1/admin/horarios",
                "/api/v1/admin/tickets",
                "/api/v1/admin/resenias"
            ],
            ValidateTokenWhenPresent = true
        },
        new()
        {
            Name = "atracciones-admin",
            ServiceKey = "AtraccionesUrl",
            Prefixes =
            [
                "/api/v1/admin/atracciones",
                "/api/v1/admin/catalogos",
                "/api/v1/admin/horarios",
                "/api/v1/admin/tickets",
                "/api/v1/admin/resenias"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["ADMIN"]
        },
        new()
        {
            Name = "facturacion",
            ServiceKey = "FacturacionUrl",
            Prefixes =
            [
                "/api/v1/me/datos-facturacion",
                "/api/v1/pagos",
                "/api/v1/facturas",
                "/api/v1/admin/pagos",
                "/api/v1/admin/facturas"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["CLIENTE", "ADMIN"]
        },
        new()
        {
            Name = "facturacion-admin",
            ServiceKey = "FacturacionUrl",
            Prefixes =
            [
                "/api/v1/admin/pagos",
                "/api/v1/admin/facturas"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["ADMIN"]
        },
        new()
        {
            Name = "auditoria",
            ServiceKey = "AuditoriaUrl",
            Prefixes =
            [
                "/api/v1/admin/auditoria"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["ADMIN"]
        },
        new()
        {
            Name = "reservas-post",
            ServiceKey = "ReservasUrl",
            Methods = ["POST"],
            Prefixes =
            [
                "/api/v1/reservas"
            ],
            ValidateTokenWhenPresent = true,
            AllowedRoles = ["CLIENTE", "ADMIN", "BOOKING_INTEGRATION"]
        },
        new()
        {
            Name = "reservas",
            ServiceKey = "ReservasUrl",
            Methods = ["GET", "PUT", "DELETE", "PATCH"],
            Prefixes =
            [
                "/api/v1/reservas",
                "/api/v1/admin/reservas",
                "/api/v1/admin/clientes",
                "/api/v1/me"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["CLIENTE", "ADMIN"]
        },
        new()
        {
            Name = "reservas-admin",
            ServiceKey = "ReservasUrl",
            Prefixes =
            [
                "/api/v1/admin/reservas",
                "/api/v1/admin/clientes"
            ],
            RequiresAuthentication = true,
            AllowedRoles = ["ADMIN"]
        }
    ];
}
