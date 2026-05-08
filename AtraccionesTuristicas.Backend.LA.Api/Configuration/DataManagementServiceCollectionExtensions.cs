using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

namespace AtraccionesTuristicas.Backend.LA.Api.Configuration;

public static class DataManagementServiceCollectionExtensions
{
    public static IServiceCollection AddDataManagementLayer(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        services.AddScoped<IClienteDataService, ClienteDataService>();
        services.AddScoped<IDatosFacturacionDataService, DatosFacturacionDataService>();
        services.AddScoped<IRolDataService, RolDataService>();
        services.AddScoped<IUsuarioDataService, UsuarioDataService>();
        services.AddScoped<IUsuarioRolDataService, UsuarioRolDataService>();
        services.AddScoped<IAuditoriaLogDataService, AuditoriaLogDataService>();

        services.AddScoped<IAtraccionDataService, AtraccionDataService>();
        services.AddScoped<ICategoriaDataService, CategoriaDataService>();
        services.AddScoped<IDestinoDataService, DestinoDataService>();
        services.AddScoped<IIdiomaDataService, IdiomaDataService>();
        services.AddScoped<IImagenDataService, ImagenDataService>();
        services.AddScoped<IIncluyeDataService, IncluyeDataService>();
        services.AddScoped<ICategoriaAtraccionDataService, CategoriaAtraccionDataService>();
        services.AddScoped<IIdiomaAtraccionDataService, IdiomaAtraccionDataService>();
        services.AddScoped<IImagenAtraccionDataService, ImagenAtraccionDataService>();
        services.AddScoped<IAtraccionIncluyeDataService, AtraccionIncluyeDataService>();

        services.AddScoped<ITicketDataService, TicketDataService>();
        services.AddScoped<IHorarioDataService, HorarioDataService>();
        services.AddScoped<IReservaDataService, ReservaDataService>();
        services.AddScoped<IReservaDetalleDataService, ReservaDetalleDataService>();
        services.AddScoped<IReservaEstadoHistorialDataService, ReservaEstadoHistorialDataService>();
        services.AddScoped<IPagoDataService, PagoDataService>();
        services.AddScoped<IFacturaDataService, FacturaDataService>();
        services.AddScoped<IReseniaDataService, ReseniaDataService>();

        return services;
    }
}
