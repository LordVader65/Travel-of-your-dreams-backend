namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static RolResponse Rol(RolDataModel x) => new() { Id = x.Id, Guid = x.Guid, Descripcion = x.Descripcion, Estado = x.Estado };
}

