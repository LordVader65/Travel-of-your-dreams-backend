using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;

public static class AtraccionDataMapper
{
    public static AtraccionDataModel ToDataModel(AtraccionEntity entity) => new()
    {
        Id = entity.at_id,
        Guid = entity.at_guid,
        DestinoId = entity.des_id,
        NumeroEstablecimiento = entity.at_num_establecimiento,
        Nombre = entity.at_nombre,
        Descripcion = entity.at_descripcion,
        Direccion = entity.at_direccion,
        DuracionMinutos = entity.at_duracion_minutos,
        PuntoEncuentro = entity.at_punto_encuentro,
        PrecioReferencia = entity.at_precio_referencia,
        IncluyeAcompaniante = entity.at_incluye_acompaniante,
        IncluyeTransporte = entity.at_incluye_transporte,
        Disponible = entity.at_disponible,
        FreeCancellation = entity.at_free_cancellation,
        SkipTheLine = entity.at_skip_the_line,
        Estado = entity.at_estado,
        UsuarioIngreso = entity.at_usuario_ingreso,
        IpIngreso = entity.at_ip_ingreso
    };

    public static AtraccionEntity ToEntity(AtraccionDataModel model) => new()
    {
        at_id = model.Id,
        at_guid = model.Guid,
        des_id = model.DestinoId,
        at_num_establecimiento = model.NumeroEstablecimiento,
        at_nombre = model.Nombre,
        at_descripcion = model.Descripcion,
        at_direccion = model.Direccion,
        at_duracion_minutos = model.DuracionMinutos,
        at_punto_encuentro = model.PuntoEncuentro,
        at_precio_referencia = model.PrecioReferencia,
        at_incluye_acompaniante = model.IncluyeAcompaniante,
        at_incluye_transporte = model.IncluyeTransporte,
        at_disponible = model.Disponible,
        at_free_cancellation = model.FreeCancellation,
        at_skip_the_line = model.SkipTheLine,
        at_estado = model.Estado,
        at_usuario_ingreso = model.UsuarioIngreso,
        at_ip_ingreso = model.IpIngreso
    };

    public static AtraccionPublicaDataModel ToPublicDataModel(AtraccionEntity entity) => new()
    {
        Id = entity.at_id,
        Guid = entity.at_guid,
        Nombre = entity.at_nombre,
        Descripcion = entity.at_descripcion,
        Pais = entity.Destino?.des_pais,
        Direccion = entity.at_direccion,
        DuracionMinutos = entity.at_duracion_minutos,
        PrecioReferencia = entity.at_precio_referencia,
        Disponible = entity.at_disponible,
        FreeCancellation = entity.at_free_cancellation,
        SkipTheLine = entity.at_skip_the_line,
        TotalResenias = entity.at_total_resenias
    };
}
