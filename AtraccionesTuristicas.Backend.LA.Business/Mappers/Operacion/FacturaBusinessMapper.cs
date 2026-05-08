namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static FacturaResponse Factura(FacturaDataModel x) => new() { Id = x.Id, Guid = x.Guid, ReservaId = x.ReservaId, PagoId = x.PagoId, DatosFacturacionId = x.DatosFacturacionId, Numero = x.Numero, FechaEmision = x.FechaEmision, Subtotal = x.Subtotal, ValorIva = x.ValorIva, Total = x.Total, Moneda = x.Moneda, Observacion = x.Observacion, Estado = x.Estado };
}

