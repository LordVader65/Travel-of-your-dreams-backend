using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using TravelDreams.Grpc.Reservas;
using TravelDreams.MsFacturacion.Business.Interfaces;
using TravelDreams.MsFacturacion.Business.Services;

namespace TravelDreams.MsFacturacion.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddFacturacionBusiness(this IServiceCollection services)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        services.AddScoped<IDatosFacturacionService, DatosFacturacionService>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<IFacturaService, FacturaService>();
        services.AddScoped(provider =>
        {
            var configuration = provider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var baseUrl = configuration["Services:ReservasGrpcUrl"] ?? configuration["Services:ReservasUrl"] ?? "http://localhost:5103";
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            return new ReservasInternal.ReservasInternalClient(GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions { HttpHandler = handler }));
        });
        services.AddScoped<IReservasIntegrationClient, ReservasGrpcClient>();
        return services;
    }
}
