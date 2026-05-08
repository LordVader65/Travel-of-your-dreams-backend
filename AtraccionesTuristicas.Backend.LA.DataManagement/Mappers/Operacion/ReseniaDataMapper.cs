using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class ReseniaDataMapper
{
    public static ReseniaDataModel ToDataModel(ReseniaEntity entity) => new()
    {
        Id = entity.rsn_id, Guid = entity.rsn_guid, AtraccionId = entity.at_id, ReservaId = entity.rev_id,
        Comentario = entity.rsn_comentario, Rating = entity.rsn_rating, FechaCreacion = entity.rsn_fecha_creacion,
        Estado = entity.rsn_estado, UsuarioCreacion = entity.rsn_usuario_creacion, IpCreacion = entity.rsn_ip_creacion
    };

    public static ReseniaEntity ToEntity(ReseniaDataModel model) => new()
    {
        rsn_id = model.Id, rsn_guid = model.Guid, at_id = model.AtraccionId, rev_id = model.ReservaId,
        rsn_comentario = model.Comentario, rsn_rating = model.Rating, rsn_fecha_creacion = model.FechaCreacion,
        rsn_estado = model.Estado, rsn_usuario_creacion = model.UsuarioCreacion, rsn_ip_creacion = model.IpCreacion
    };
}
