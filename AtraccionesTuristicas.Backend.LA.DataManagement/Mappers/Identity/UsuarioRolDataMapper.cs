using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Identity;

public static class UsuarioRolDataMapper
{
    public static UsuarioRolDataModel ToDataModel(UsuarioRolEntity entity) => new()
    {
        Id = entity.usu_rol_id, UsuarioId = entity.usu_id, RolId = entity.rol_id, Estado = entity.usu_rol_estado
    };

    public static UsuarioRolEntity ToEntity(UsuarioRolDataModel model) => new()
    {
        usu_rol_id = model.Id, usu_id = model.UsuarioId, rol_id = model.RolId, usu_rol_estado = model.Estado
    };
}
