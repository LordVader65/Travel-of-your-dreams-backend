namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static HorarioResponse Horario(HorarioDataModel x) => new() { Id = x.Id, Guid = x.Guid, AtraccionId = x.AtraccionId, Fecha = x.Fecha, HoraInicio = x.HoraInicio, HoraFin = x.HoraFin, CuposDisponibles = x.CuposDisponibles, DiasSemana = x.DiasSemana, Estado = x.Estado };
}

