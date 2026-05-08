using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class ReservaDataMapper
{
    public static ReservaDataModel ToDataModel(ReservaEntity entity) => new()
    {
        Id = entity.rev_id,
        Guid = entity.rev_guid,
        Codigo = entity.rev_codigo,
        ClienteId = entity.cli_id,
        HorarioId = entity.hor_id,
        FechaReservaUtc = entity.rev_fecha_reserva_utc,
        FechaExpiracionUtc = entity.rev_fecha_expiracion_utc,
        Subtotal = entity.rev_subtotal,
        ValorIva = entity.rev_valor_iva,
        Total = entity.rev_total,
        Moneda = entity.rev_moneda,
        OrigenCanal = entity.rev_origen_canal,
        Estado = entity.rev_estado,
        Detalles = entity.Detalles.Select(ReservaDetalleDataMapper.ToDataModel).ToList()
    };
}
