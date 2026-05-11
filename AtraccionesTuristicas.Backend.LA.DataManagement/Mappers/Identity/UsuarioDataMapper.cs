using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Identity;

public static class UsuarioDataMapper
{
    public static UsuarioDataModel ToDataModel(UsuarioEntity entity) => new()
    {
        Id = entity.usu_id,
        Guid = entity.usu_guid,
        Login = entity.usu_login,
        PasswordHash = entity.usu_password_hash,
        Estado = entity.usu_estado,
        Roles = entity.UsuarioRoles
            .Where(x => x.usu_rol_estado == "A" && x.Rol is not null)
            .Select(x => x.Rol!.rol_descripcion)
            .ToList(),
        ClienteId = entity.Clientes.FirstOrDefault()?.cli_id,
        ClienteGuid = entity.Clientes.FirstOrDefault()?.cli_guid,
        ClienteNombre = NombreCliente(entity),
        ClienteIdentificacion = entity.Clientes.FirstOrDefault()?.cli_numero_identificacion,
        ClienteCorreo = entity.Clientes.FirstOrDefault()?.cli_correo
    };

    private static string? NombreCliente(UsuarioEntity entity)
    {
        var cliente = entity.Clientes.FirstOrDefault();
        if (cliente is null)
        {
            return null;
        }

        var nombres = string.Join(' ', new[] { cliente.cli_nombres, cliente.cli_apellidos }.Where(x => !string.IsNullOrWhiteSpace(x)));
        return !string.IsNullOrWhiteSpace(nombres) ? nombres : cliente.cli_razon_social;
    }
}
