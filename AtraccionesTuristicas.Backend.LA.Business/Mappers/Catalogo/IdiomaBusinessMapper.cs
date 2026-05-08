namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static IdiomaResponse Idioma(IdiomaDataModel x) => new() { Id = x.Id, Guid = x.Guid, Codigo = x.Codigo, Descripcion = x.Descripcion, Estado = x.Estado };
        public static IdiomaDataModel Idioma(CrearIdiomaRequest x) => new() { Codigo = x.Codigo, Descripcion = x.Descripcion, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
}

