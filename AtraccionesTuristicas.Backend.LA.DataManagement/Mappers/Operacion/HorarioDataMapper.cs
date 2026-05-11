using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class HorarioDataMapper
{
    public static HorarioDataModel ToDataModel(HorarioEntity entity) => new()
    {
        Id = entity.hor_id,
        Guid = entity.hor_guid,
        AtraccionId = entity.at_id,
        Fecha = entity.hor_fecha,
        HoraInicio = entity.hor_hora_inicio,
        HoraFin = entity.hor_hora_fin,
        CuposDisponibles = entity.hor_cupos_disponibles,
        DiasSemana = entity.hor_dias_semana,
        Estado = entity.hor_estado
    };
}
