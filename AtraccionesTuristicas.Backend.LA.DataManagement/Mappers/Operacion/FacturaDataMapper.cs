using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class FacturaDataMapper
{
    public static FacturaDataModel ToDataModel(FacturaEntity entity) => new()
    {
        Id = entity.fac_id,
        Guid = entity.fac_guid,
        ReservaId = entity.rev_id,
        PagoId = entity.pag_id,
        DatosFacturacionId = entity.dfac_id,
        Numero = entity.fac_numero,
        FechaEmision = entity.fac_fecha_emision,
        Subtotal = entity.fac_subtotal,
        ValorIva = entity.fac_valor_iva,
        Total = entity.fac_total,
        Moneda = entity.fac_moneda,
        Observacion = entity.fac_observacion,
        Estado = entity.fac_estado
    };
}
