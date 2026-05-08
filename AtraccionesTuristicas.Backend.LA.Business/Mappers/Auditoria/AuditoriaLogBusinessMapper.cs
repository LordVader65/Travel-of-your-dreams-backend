namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static AuditoriaLogResponse Auditoria(AuditoriaLogDataModel x) => new() { Id = x.Id, Guid = x.Guid, Tabla = x.Tabla, Operacion = x.Operacion, RegistroId = x.RegistroId, RegistroGuid = x.RegistroGuid, DatosAnteriores = x.DatosAnteriores, DatosNuevos = x.DatosNuevos, FechaUtc = x.FechaUtc, Usuario = x.Usuario, Ip = x.Ip, OrigenCanal = x.OrigenCanal };
}

