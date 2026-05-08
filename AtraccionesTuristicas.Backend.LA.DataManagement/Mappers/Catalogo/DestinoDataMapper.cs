using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;

public static class DestinoDataMapper
{
    public static DestinoDataModel ToDataModel(DestinoEntity entity) => new()
    {
        Id = entity.des_id, Guid = entity.des_guid, Nombre = entity.des_nombre, Pais = entity.des_pais,
        ImagenUrl = entity.des_imagen_url, Estado = entity.des_estado, UsuarioIngreso = entity.des_usuario_ingreso,
        IpIngreso = entity.des_ip_ingreso
    };

    public static DestinoEntity ToEntity(DestinoDataModel model) => new()
    {
        des_id = model.Id, des_guid = model.Guid, des_nombre = model.Nombre, des_pais = model.Pais,
        des_imagen_url = model.ImagenUrl, des_estado = model.Estado, des_usuario_ingreso = model.UsuarioIngreso,
        des_ip_ingreso = model.IpIngreso
    };
}
