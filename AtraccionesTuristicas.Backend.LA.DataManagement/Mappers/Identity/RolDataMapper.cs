using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Identity;

public static class RolDataMapper
{
    public static RolDataModel ToDataModel(RolEntity entity) => new()
    {
        Id = entity.rol_id, Guid = entity.rol_guid, Descripcion = entity.rol_descripcion,
        Estado = entity.rol_estado, UsuarioIngreso = entity.rol_usuario_ingreso, IpIngreso = entity.rol_ip_ingreso
    };

    public static RolEntity ToEntity(RolDataModel model) => new()
    {
        rol_id = model.Id, rol_guid = model.Guid, rol_descripcion = model.Descripcion,
        rol_estado = model.Estado, rol_usuario_ingreso = model.UsuarioIngreso, rol_ip_ingreso = model.IpIngreso
    };
}
