namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static ImagenResponse Imagen(ImagenDataModel x) => new() { Id = x.Id, Guid = x.Guid, Url = x.Url, Descripcion = x.Descripcion, Estado = x.Estado };
        public static ImagenDataModel Imagen(CrearImagenRequest x) => new() { Url = x.Url, Descripcion = x.Descripcion, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
}

