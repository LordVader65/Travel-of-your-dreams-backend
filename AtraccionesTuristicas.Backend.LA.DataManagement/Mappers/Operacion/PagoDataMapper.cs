using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class PagoDataMapper
{
    public static PagoDataModel ToDataModel(PagoEntity entity) => new()
    {
        Id = entity.pag_id,
        Guid = entity.pag_guid,
        ReservaId = entity.rev_id,
        Referencia = entity.pag_referencia,
        Metodo = entity.pag_metodo,
        Monto = entity.pag_monto,
        Moneda = entity.pag_moneda,
        FechaUtc = entity.pag_fecha_utc,
        Estado = entity.pag_estado
    };
}
