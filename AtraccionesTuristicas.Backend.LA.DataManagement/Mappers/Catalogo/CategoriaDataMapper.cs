using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;

public static class CategoriaDataMapper
{
    public static CategoriaDataModel ToDataModel(CategoriaEntity entity) => new()
    {
        Id = entity.cat_id, Guid = entity.cat_guid, ParentId = entity.cat_parent_id, Nombre = entity.cat_nombre,
        TagName = entity.cat_tagname, Estado = entity.cat_estado, UsuarioIngreso = entity.cat_usuario_ingreso,
        IpIngreso = entity.cat_ip_ingreso
    };

    public static CategoriaEntity ToEntity(CategoriaDataModel model) => new()
    {
        cat_id = model.Id, cat_guid = model.Guid, cat_parent_id = model.ParentId, cat_nombre = model.Nombre,
        cat_tagname = model.TagName, cat_estado = model.Estado, cat_usuario_ingreso = model.UsuarioIngreso,
        cat_ip_ingreso = model.IpIngreso
    };
}
