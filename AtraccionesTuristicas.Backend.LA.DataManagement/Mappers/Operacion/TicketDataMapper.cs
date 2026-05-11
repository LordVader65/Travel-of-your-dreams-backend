using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;

public static class TicketDataMapper
{
    public static TicketDataModel ToDataModel(TicketEntity entity) => new()
    {
        Id = entity.tck_id, Guid = entity.tck_guid, AtraccionId = entity.at_id, Titulo = entity.tck_titulo,
        Precio = entity.tck_precio, Moneda = entity.tck_moneda, TipoParticipante = entity.tck_tipo_participante,
        CapacidadMaxima = entity.tck_capacidad_maxima, Estado = entity.tck_estado,
        UsuarioIngreso = entity.tck_usuario_ingreso, IpIngreso = entity.tck_ip_ingreso
    };

    public static TicketEntity ToEntity(TicketDataModel model) => new()
    {
        tck_id = model.Id, tck_guid = model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid, at_id = model.AtraccionId, tck_titulo = model.Titulo,
        tck_precio = model.Precio, tck_moneda = model.Moneda, tck_tipo_participante = model.TipoParticipante,
        tck_capacidad_maxima = model.CapacidadMaxima, tck_estado = model.Estado,
        tck_usuario_ingreso = model.UsuarioIngreso, tck_ip_ingreso = model.IpIngreso
    };
}
