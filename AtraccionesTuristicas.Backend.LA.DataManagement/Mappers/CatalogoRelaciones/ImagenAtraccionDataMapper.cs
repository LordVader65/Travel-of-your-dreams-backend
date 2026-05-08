using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;

public static class ImagenAtraccionDataMapper
{
    public static ImagenAtraccionDataModel ToDataModel(ImagenAtraccionEntity entity) => new()
    {
        ImagenId = entity.img_id, AtraccionId = entity.at_id, EsPrincipal = entity.ima_es_principal,
        Orden = entity.ima_orden, Estado = entity.ima_estado, UsuarioIngreso = entity.ima_usuario_ingreso
    };

    public static ImagenAtraccionEntity ToEntity(ImagenAtraccionDataModel model) => new()
    {
        img_id = model.ImagenId, at_id = model.AtraccionId, ima_es_principal = model.EsPrincipal,
        ima_orden = model.Orden, ima_estado = model.Estado, ima_usuario_ingreso = model.UsuarioIngreso
    };
}
