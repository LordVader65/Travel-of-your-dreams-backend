namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static PagoResponse Pago(PagoDataModel x) => new() { Id = x.Id, Guid = x.Guid, ReservaId = x.ReservaId, Referencia = x.Referencia, Metodo = x.Metodo, Monto = x.Monto, Moneda = x.Moneda, FechaUtc = x.FechaUtc, Estado = x.Estado };
}

