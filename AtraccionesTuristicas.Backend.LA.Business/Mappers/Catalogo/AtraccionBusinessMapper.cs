namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static AtraccionResponse Atraccion(AtraccionDataModel x) => new() { Id = x.Id, Guid = x.Guid, DestinoId = x.DestinoId, NumeroEstablecimiento = x.NumeroEstablecimiento, Nombre = x.Nombre, Descripcion = x.Descripcion, Direccion = x.Direccion, DuracionMinutos = x.DuracionMinutos, PuntoEncuentro = x.PuntoEncuentro, PrecioReferencia = x.PrecioReferencia, IncluyeAcompaniante = x.IncluyeAcompaniante, IncluyeTransporte = x.IncluyeTransporte, Disponible = x.Disponible, FreeCancellation = x.FreeCancellation, SkipTheLine = x.SkipTheLine, Estado = x.Estado, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso };
        public static AtraccionDataModel Atraccion(CrearAtraccionRequest x) => new() { DestinoId = x.DestinoId, NumeroEstablecimiento = x.NumeroEstablecimiento, Nombre = x.Nombre, Descripcion = x.Descripcion, Direccion = x.Direccion, DuracionMinutos = x.DuracionMinutos, PuntoEncuentro = x.PuntoEncuentro, PrecioReferencia = x.PrecioReferencia, IncluyeAcompaniante = x.IncluyeAcompaniante, IncluyeTransporte = x.IncluyeTransporte, Disponible = x.Disponible, FreeCancellation = x.FreeCancellation, SkipTheLine = x.SkipTheLine, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
        public static AtraccionDataModel Atraccion(ActualizarAtraccionRequest x) { var m = Atraccion((CrearAtraccionRequest)x); m.Guid = x.Guid; m.Estado = x.Estado; return m; }
        public static AtraccionPublicaResponse AtraccionPublica(AtraccionPublicaDataModel x) => new() { Id = x.Id, Guid = x.Guid, Nombre = x.Nombre, Descripcion = x.Descripcion, Pais = x.Pais, Direccion = x.Direccion, DuracionMinutos = x.DuracionMinutos, PrecioReferencia = x.PrecioReferencia, Disponible = x.Disponible, FreeCancellation = x.FreeCancellation, SkipTheLine = x.SkipTheLine, TotalResenias = x.TotalResenias };
}

