using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;

public static class IncluyeDataMapper
{
    public static IncluyeDataModel ToDataModel(IncluyeEntity entity) => new()
    {
        Id = entity.inc_id, Guid = entity.inc_guid, Descripcion = entity.inc_descripcion,
        Tipo = entity.inc_tipo, Estado = entity.inc_estado
    };

    public static IncluyeEntity ToEntity(IncluyeDataModel model) => new()
    {
        inc_id = model.Id, inc_guid = model.Guid, inc_descripcion = model.Descripcion,
        inc_tipo = model.Tipo, inc_estado = model.Estado
    };
}
