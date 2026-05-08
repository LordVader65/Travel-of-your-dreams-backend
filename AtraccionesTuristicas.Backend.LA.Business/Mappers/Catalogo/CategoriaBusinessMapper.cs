namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static CategoriaResponse Categoria(CategoriaDataModel x) => new() { Id = x.Id, Guid = x.Guid, ParentId = x.ParentId, Nombre = x.Nombre, TagName = x.TagName, Estado = x.Estado };
        public static CategoriaDataModel Categoria(CrearCategoriaRequest x) => new() { ParentId = x.ParentId, Nombre = x.Nombre, TagName = x.TagName, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
}

