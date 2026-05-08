using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;

public static class AtraccionIncluyeDataMapper
{
    public static AtraccionIncluyeDataModel ToDataModel(AtraccionIncluyeEntity entity) => new()
    {
        IncluyeId = entity.inc_id, AtraccionId = entity.at_id, Estado = entity.ai_estado,
        UsuarioIngreso = entity.ai_usuario_ingreso
    };

    public static AtraccionIncluyeEntity ToEntity(AtraccionIncluyeDataModel model) => new()
    {
        inc_id = model.IncluyeId, at_id = model.AtraccionId, ai_estado = model.Estado,
        ai_usuario_ingreso = model.UsuarioIngreso
    };
}
