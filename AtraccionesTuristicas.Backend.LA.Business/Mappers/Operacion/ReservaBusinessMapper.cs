namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static ReservaResponse Reserva(ReservaDataModel x) => new() { Id = x.Id, Guid = x.Guid, Codigo = x.Codigo, ClienteId = x.ClienteId, HorarioId = x.HorarioId, FechaReservaUtc = x.FechaReservaUtc, FechaExpiracionUtc = x.FechaExpiracionUtc, Subtotal = x.Subtotal, ValorIva = x.ValorIva, Total = x.Total, Moneda = x.Moneda, Estado = x.Estado, Detalles = x.Detalles.Select(ReservaDetalle).ToList() };
        public static ReservaDetalleResponse ReservaDetalle(ReservaDetalleDataModel x) => new() { Id = x.Id, Guid = x.Guid, ReservaId = x.ReservaId, TicketId = x.TicketId, Cantidad = x.Cantidad, PrecioUnitario = x.PrecioUnitario, Subtotal = x.Subtotal, Estado = x.Estado };
}

