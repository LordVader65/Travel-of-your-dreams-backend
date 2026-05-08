namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
    public static ImagenAtraccionResponse ImagenAtraccion(ImagenAtraccionDataModel x) => new() { ImagenId = x.ImagenId, AtraccionId = x.AtraccionId, EsPrincipal = x.EsPrincipal, Orden = x.Orden, Estado = x.Estado };
}
