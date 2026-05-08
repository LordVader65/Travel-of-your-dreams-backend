using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;

public static class IdiomaAtraccionDataMapper
{
    public static IdiomaAtraccionDataModel ToDataModel(IdiomaAtraccionEntity entity) => new()
    {
        IdiomaId = entity.id_id, AtraccionId = entity.at_id, Estado = entity.ia_estado,
        UsuarioIngreso = entity.ia_usuario_ingreso
    };

    public static IdiomaAtraccionEntity ToEntity(IdiomaAtraccionDataModel model) => new()
    {
        id_id = model.IdiomaId, at_id = model.AtraccionId, ia_estado = model.Estado,
        ia_usuario_ingreso = model.UsuarioIngreso
    };
}
