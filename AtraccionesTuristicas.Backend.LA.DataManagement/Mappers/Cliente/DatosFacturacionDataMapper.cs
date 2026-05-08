using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Cliente;

public static class DatosFacturacionDataMapper
{
    public static DatosFacturacionDataModel ToDataModel(DatosFacturacionEntity entity) => new()
    {
        Id = entity.dfac_id, Guid = entity.dfac_guid, ClienteId = entity.cli_id,
        TipoIdentificacion = entity.dfac_tipo_identificacion, NumeroIdentificacion = entity.dfac_numero_identificacion,
        RazonSocial = entity.dfac_razon_social, Nombre = entity.dfac_nombre, Apellido = entity.dfac_apellido,
        Correo = entity.dfac_correo, Telefono = entity.dfac_telefono, Direccion = entity.dfac_direccion,
        Estado = entity.dfac_estado, UsuarioIngreso = entity.dfac_usuario_ingreso, IpIngreso = entity.dfac_ip_ingreso
    };

    public static DatosFacturacionEntity ToEntity(DatosFacturacionDataModel model) => new()
    {
        dfac_id = model.Id, dfac_guid = model.Guid, cli_id = model.ClienteId,
        dfac_tipo_identificacion = model.TipoIdentificacion, dfac_numero_identificacion = model.NumeroIdentificacion,
        dfac_razon_social = model.RazonSocial, dfac_nombre = model.Nombre, dfac_apellido = model.Apellido,
        dfac_correo = model.Correo, dfac_telefono = model.Telefono, dfac_direccion = model.Direccion,
        dfac_estado = model.Estado, dfac_usuario_ingreso = model.UsuarioIngreso, dfac_ip_ingreso = model.IpIngreso
    };
}
