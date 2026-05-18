using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Interfaces;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.Business.Services;

public sealed class FacturaService : IFacturaService
{
    private readonly IFacturacionDataService _data;
    private readonly IReservasIntegrationClient _reservas;

    public FacturaService(IFacturacionDataService data, IReservasIntegrationClient reservas)
    {
        _data = data;
        _reservas = reservas;
    }

    public async Task<PagedResponse<FacturaResponse>> ListarAsync(FacturaFiltroRequest filtro, CancellationToken ct = default) =>
        FacturacionMappers.Page(await _data.ListarFacturasAsync(new FacturaFiltroDataModel
        {
            ClienteGuid = filtro.ClienteGuid,
            ReservaGuid = filtro.ReservaGuid,
            Numero = filtro.Numero,
            Estado = filtro.Estado,
            FechaDesdeUtc = filtro.FechaDesdeUtc,
            FechaHastaUtc = filtro.FechaHastaUtc,
            Page = filtro.Page,
            Limit = filtro.Limit
        }, ct), FacturacionMappers.Factura);

    public async Task<FacturaResponse?> ObtenerAsync(Guid guid, CancellationToken ct = default)
    {
        var model = await _data.ObtenerFacturaPorGuidAsync(guid, ct);
        return model is null ? null : FacturacionMappers.Factura(model);
    }

    public async Task<FacturaResponse?> ObtenerPorNumeroAsync(string numero, CancellationToken ct = default)
    {
        var model = await _data.ObtenerFacturaPorNumeroAsync(numero, ct);
        return model is null ? null : FacturacionMappers.Factura(model);
    }

    public async Task<FacturaResponse> GenerarAsync(GenerarFacturaRequest request, CancellationToken ct = default)
    {
        if (request.ReservaGuid == Guid.Empty) throw new InvalidOperationException("ReservaGuid es obligatorio.");

        var reserva = await _reservas.GetPaymentSnapshotAsync(request.ReservaGuid, ct)
            ?? throw new InvalidOperationException("Reserva no encontrada.");

        if (reserva.Estado is not ("PAGADA" or "CONFIRMADA"))
        {
            throw new InvalidOperationException("La reserva debe estar PAGADA o CONFIRMADA para facturar.");
        }

        var factura = await _data.GenerarFacturaDesdePagoAprobadoAsync(
            reserva.ReservaGuid,
            reserva.ClienteGuid,
            request.DatosFacturacionGuid,
            reserva.Subtotal,
            reserva.ValorIva,
            reserva.Total,
            reserva.Moneda,
            "admin",
            "api",
            request.Observacion,
            ct);

        return FacturacionMappers.Factura(factura);
    }
}
