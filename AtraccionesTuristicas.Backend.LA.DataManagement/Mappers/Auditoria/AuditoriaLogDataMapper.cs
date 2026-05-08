using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Auditoria;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Auditoria;

public static class AuditoriaLogDataMapper
{
    public static AuditoriaLogDataModel ToDataModel(AuditoriaLogEntity entity) => new()
    {
        Id = entity.log_id,
        Guid = entity.log_guid,
        Tabla = entity.log_tabla,
        Operacion = entity.log_operacion,
        RegistroId = entity.log_registro_id,
        RegistroGuid = entity.log_registro_guid,
        DatosAnteriores = entity.log_datos_anteriores,
        DatosNuevos = entity.log_datos_nuevos,
        FechaUtc = entity.log_fecha_utc,
        Usuario = entity.log_usuario,
        Ip = entity.log_ip,
        OrigenCanal = entity.log_origen_canal
    };
}
