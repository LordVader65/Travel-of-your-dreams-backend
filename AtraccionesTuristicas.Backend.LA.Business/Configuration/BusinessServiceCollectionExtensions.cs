using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auditoria;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auth;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Identity;
using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.Business.Services.Auditoria;
using AtraccionesTuristicas.Backend.LA.Business.Services.Auth;
using AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;
using AtraccionesTuristicas.Backend.LA.Business.Services.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.Business.Services.Cliente;
using AtraccionesTuristicas.Backend.LA.Business.Services.Identity;
using AtraccionesTuristicas.Backend.LA.Business.Services.Operacion;
using Microsoft.Extensions.DependencyInjection;

namespace AtraccionesTuristicas.Backend.LA.Business.Configuration;

public static class BusinessServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddSingleton(jwtOptions);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IDatosFacturacionService, DatosFacturacionService>();

        services.AddScoped<IAtraccionService, AtraccionService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IDestinoService, DestinoService>();
        services.AddScoped<IIdiomaService, IdiomaService>();
        services.AddScoped<IImagenService, ImagenService>();
        services.AddScoped<IIncluyeService, IncluyeService>();

        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IHorarioService, HorarioService>();
        services.AddScoped<IReservaService, ReservaService>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<IFacturaService, FacturaService>();
        services.AddScoped<IReseniaService, ReseniaService>();

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRolService, RolService>();
        services.AddScoped<IAuditoriaLogService, AuditoriaLogService>();

        services.AddScoped<ICategoriaAtraccionService, CategoriaAtraccionService>();
        services.AddScoped<IIdiomaAtraccionService, IdiomaAtraccionService>();
        services.AddScoped<IImagenAtraccionService, ImagenAtraccionService>();
        services.AddScoped<IAtraccionIncluyeService, AtraccionIncluyeService>();

        return services;
    }
}
