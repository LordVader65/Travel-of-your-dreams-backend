namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static IncluyeResponse Incluye(IncluyeDataModel x) => new() { Id = x.Id, Guid = x.Guid, Descripcion = x.Descripcion, Tipo = x.Tipo, Estado = x.Estado };
        public static IncluyeDataModel Incluye(CrearIncluyeRequest x) => new() { Descripcion = x.Descripcion, Tipo = x.Tipo, Estado = BusinessConstants.EstadoActivo };
}

