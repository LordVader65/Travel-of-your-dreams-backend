namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static ReseniaResponse Resenia(ReseniaDataModel x) => new() { Id = x.Id, Guid = x.Guid, AtraccionId = x.AtraccionId, ReservaId = x.ReservaId, Comentario = x.Comentario, Rating = x.Rating, FechaCreacion = x.FechaCreacion, Estado = x.Estado };
}

