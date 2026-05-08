namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static DestinoResponse Destino(DestinoDataModel x) => new() { Id = x.Id, Guid = x.Guid, Nombre = x.Nombre, Pais = x.Pais, ImagenUrl = x.ImagenUrl, Estado = x.Estado };
        public static DestinoDataModel Destino(CrearDestinoRequest x) => new() { Nombre = x.Nombre, Pais = x.Pais, ImagenUrl = x.ImagenUrl, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
}

