using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.DataManagement.Interfaces;

public interface IFacturacionDataService
{
    Task<FacturaDataModel?> ObtenerFacturaPorGuidAsync(Guid guid, CancellationToken ct = default);
    Task<FacturaDataModel?> ObtenerFacturaPorNumeroAsync(string numero, CancellationToken ct = default);
    Task<PagedResult<FacturaDataModel>> ListarFacturasAsync(FacturaFiltroDataModel filtro, CancellationToken ct = default);
    Task<FacturaDataModel> RegistrarPagoYFacturaAsync(CrearPagoDataModel pago, decimal subtotal, decimal valorIva, CancellationToken ct = default);
    Task<FacturaDataModel> GenerarFacturaDesdePagoAprobadoAsync(Guid reservaGuid, Guid clienteGuid, Guid? datosFacturacionGuid, decimal subtotal, decimal valorIva, decimal total, string moneda, string usuario, string ip, string? observacion, CancellationToken ct = default);
}
