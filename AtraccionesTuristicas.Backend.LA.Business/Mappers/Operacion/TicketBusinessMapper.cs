namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static TicketResponse Ticket(TicketDataModel x) => new() { Id = x.Id, Guid = x.Guid, AtraccionId = x.AtraccionId, Titulo = x.Titulo, Precio = x.Precio, Moneda = x.Moneda, TipoParticipante = x.TipoParticipante, CapacidadMaxima = x.CapacidadMaxima, Estado = x.Estado };
        public static TicketDataModel Ticket(CrearTicketRequest x) => new() { AtraccionId = x.AtraccionId, Titulo = x.Titulo, Precio = x.Precio, Moneda = x.Moneda, TipoParticipante = x.TipoParticipante, CapacidadMaxima = x.CapacidadMaxima, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
}

