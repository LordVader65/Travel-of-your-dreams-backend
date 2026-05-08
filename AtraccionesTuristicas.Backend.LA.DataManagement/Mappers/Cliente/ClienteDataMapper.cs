using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Cliente;

public static class ClienteDataMapper
{
    public static ClienteDataModel ToDataModel(ClienteEntity entity) => new()
    {
        Id = entity.cli_id,
        Guid = entity.cli_guid,
        UsuarioId = entity.usu_id,
        TipoIdentificacion = entity.cli_tipo_identificacion,
        NumeroIdentificacion = entity.cli_numero_identificacion,
        Nombres = entity.cli_nombres,
        Apellidos = entity.cli_apellidos,
        RazonSocial = entity.cli_razon_social,
        Correo = entity.cli_correo,
        Telefono = entity.cli_telefono,
        Direccion = entity.cli_direccion,
        Estado = entity.cli_estado,
        RowVersion = entity.cli_row_version,
        UsuarioIngreso = entity.cli_usuario_ingreso,
        IpIngreso = entity.cli_ip_ingreso
    };

    public static ClienteEntity ToEntity(ClienteDataModel model) => new()
    {
        cli_id = model.Id,
        cli_guid = model.Guid,
        usu_id = model.UsuarioId,
        cli_tipo_identificacion = model.TipoIdentificacion,
        cli_numero_identificacion = model.NumeroIdentificacion,
        cli_nombres = model.Nombres,
        cli_apellidos = model.Apellidos,
        cli_razon_social = model.RazonSocial,
        cli_correo = model.Correo,
        cli_telefono = model.Telefono,
        cli_direccion = model.Direccion,
        cli_estado = model.Estado,
        cli_row_version = model.RowVersion,
        cli_usuario_ingreso = model.UsuarioIngreso,
        cli_ip_ingreso = model.IpIngreso
    };
}
