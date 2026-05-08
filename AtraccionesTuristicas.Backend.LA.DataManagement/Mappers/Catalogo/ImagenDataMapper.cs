using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;

public static class ImagenDataMapper
{
    public static ImagenDataModel ToDataModel(ImagenEntity entity) => new()
    {
        Id = entity.img_id, Guid = entity.img_guid, Url = entity.img_url, Descripcion = entity.img_descripcion,
        Estado = entity.img_estado, UsuarioIngreso = entity.img_usuario_ingreso, IpIngreso = entity.img_ip_ingreso
    };

    public static ImagenEntity ToEntity(ImagenDataModel model) => new()
    {
        img_id = model.Id, img_guid = model.Guid, img_url = model.Url, img_descripcion = model.Descripcion,
        img_estado = model.Estado, img_usuario_ingreso = model.UsuarioIngreso, img_ip_ingreso = model.IpIngreso
    };
}
