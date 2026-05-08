using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;

public static class CategoriaAtraccionDataMapper
{
    public static CategoriaAtraccionDataModel ToDataModel(CategoriaAtraccionEntity entity) => new()
    {
        CategoriaId = entity.cat_id, AtraccionId = entity.at_id, Estado = entity.ca_estado,
        UsuarioIngreso = entity.ca_usuario_ingreso
    };

    public static CategoriaAtraccionEntity ToEntity(CategoriaAtraccionDataModel model) => new()
    {
        cat_id = model.CategoriaId, at_id = model.AtraccionId, ca_estado = model.Estado,
        ca_usuario_ingreso = model.UsuarioIngreso
    };
}
