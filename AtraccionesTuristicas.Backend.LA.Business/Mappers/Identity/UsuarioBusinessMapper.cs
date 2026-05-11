namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static UsuarioResponse Usuario(UsuarioDataModel x) => new()
        {
            Id = x.Id,
            Guid = x.Guid,
            Login = x.Login,
            Estado = x.Estado,
            Roles = x.Roles,
            ClienteId = x.ClienteId,
            ClienteGuid = x.ClienteGuid,
            ClienteNombre = x.ClienteNombre,
            ClienteIdentificacion = x.ClienteIdentificacion,
            ClienteCorreo = x.ClienteCorreo
        };
}

