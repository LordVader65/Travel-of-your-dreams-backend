using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;

public static class IdiomaDataMapper
{
    public static IdiomaDataModel ToDataModel(IdiomaEntity entity) => new()
    {
        Id = entity.id_id, Guid = entity.id_guid, Codigo = entity.id_codigo, Descripcion = entity.id_descripcion,
        Estado = entity.id_estado, UsuarioIngreso = entity.id_usuario_ingreso, IpIngreso = entity.id_ip_ingreso
    };

    public static IdiomaEntity ToEntity(IdiomaDataModel model) => new()
    {
        id_id = model.Id, id_guid = model.Guid, id_codigo = model.Codigo, id_descripcion = model.Descripcion,
        id_estado = model.Estado, id_usuario_ingreso = model.UsuarioIngreso, id_ip_ingreso = model.IpIngreso
    };
}
