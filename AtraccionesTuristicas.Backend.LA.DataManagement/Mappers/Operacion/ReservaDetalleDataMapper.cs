using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class ReservaDetalleDataMapper
{
    public static ReservaDetalleDataModel ToDataModel(ReservaDetalleEntity entity) => new()
    {
        Id = entity.rdet_id,
        Guid = entity.rdet_guid,
        ReservaId = entity.rev_id,
        TicketId = entity.tck_id,
        Cantidad = entity.rdet_cantidad,
        PrecioUnitario = entity.rdet_precio_unit,
        Subtotal = entity.rdet_subtotal,
        Estado = entity.rdet_estado
    };
}
