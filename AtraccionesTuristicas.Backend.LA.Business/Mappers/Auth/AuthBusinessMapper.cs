namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
    public static CurrentUserData CurrentUser(UsuarioDataModel usuario, Guid? clienteGuid = null) => new()
    {
        UsuarioGuid = usuario.Guid,
        ClienteGuid = clienteGuid,
        Login = usuario.Login,
        Roles = usuario.Roles
    };
}
