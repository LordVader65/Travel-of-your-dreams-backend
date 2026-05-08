namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
    public static AtraccionIncluyeResponse AtraccionIncluye(AtraccionIncluyeDataModel x) => new() { IncluyeId = x.IncluyeId, AtraccionId = x.AtraccionId, Estado = x.Estado };
}
