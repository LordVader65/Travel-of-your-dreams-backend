namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
    public static IdiomaAtraccionResponse IdiomaAtraccion(IdiomaAtraccionDataModel x) => new() { IdiomaId = x.IdiomaId, AtraccionId = x.AtraccionId, Estado = x.Estado };
}
