using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.DataManagement.Models;

namespace TravelDreams.MsFacturacion.Business.Services;

internal static class FacturacionMappers
{
    public static PagoResponse Pago(PagoDataModel model) => new()
    {
        Guid = model.Guid,
        ReservaGuid = model.ReservaGuid,
        ClienteGuid = model.ClienteGuid,
        DatosFacturacionGuid = model.DatosFacturacionGuid,
        Monto = model.Monto,
        Moneda = model.Moneda,
        Metodo = model.Metodo,
        Referencia = model.Referencia,
        FechaUtc = model.FechaUtc,
        OrigenCanal = model.OrigenCanal,
        Estado = model.Estado,
        Observacion = model.Observacion
    };

    public static FacturaResponse Factura(FacturaDataModel model) => new()
    {
        Guid = model.Guid,
        Numero = model.Numero,
        ReservaGuid = model.ReservaGuid,
        ClienteGuid = model.ClienteGuid,
        PagoGuid = model.PagoGuid,
        DatosFacturacionGuid = model.DatosFacturacionGuid,
        FechaEmisionUtc = model.FechaEmisionUtc,
        Subtotal = model.Subtotal,
        ValorIva = model.ValorIva,
        Total = model.Total,
        Moneda = model.Moneda,
        Observacion = model.Observacion,
        Estado = model.Estado,
        Pago = model.Pago is null ? null : Pago(model.Pago),
        DatosFacturacion = model.DatosFacturacion is null ? null : DatosFacturacionService.Map(model.DatosFacturacion)
    };

    public static PagedResponse<TOut> Page<TIn, TOut>(PagedResult<TIn> source, Func<TIn, TOut> map) => new()
    {
        Items = source.Items.Select(map).ToList(),
        Page = source.Page,
        Limit = source.Limit,
        Total = source.Total
    };
}
