using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class ReservaEstadoHistorialDataMapper
{
    public static ReservaEstadoHistorialDataModel ToDataModel(ReservaEstadoHistorialEntity entity) => new()
    {
        Id = entity.reh_id, Guid = entity.reh_guid, ReservaId = entity.rev_id,
        EstadoAnterior = entity.reh_estado_anterior, EstadoNuevo = entity.reh_estado_nuevo,
        FechaUtc = entity.reh_fecha_utc, Usuario = entity.reh_usuario, Ip = entity.reh_ip,
        Observacion = entity.reh_observacion
    };
}
