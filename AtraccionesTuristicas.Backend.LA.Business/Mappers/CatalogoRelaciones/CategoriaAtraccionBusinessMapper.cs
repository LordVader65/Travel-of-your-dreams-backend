namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
    public static CategoriaAtraccionResponse CategoriaAtraccion(CategoriaAtraccionDataModel x) => new() { CategoriaId = x.CategoriaId, AtraccionId = x.AtraccionId, Estado = x.Estado };
}
