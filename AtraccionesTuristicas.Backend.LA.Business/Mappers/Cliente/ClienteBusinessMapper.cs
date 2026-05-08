namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static ClienteResponse Cliente(ClienteDataModel x) => new() { Id = x.Id, Guid = x.Guid, UsuarioId = x.UsuarioId, TipoIdentificacion = x.TipoIdentificacion, NumeroIdentificacion = x.NumeroIdentificacion, Nombres = x.Nombres, Apellidos = x.Apellidos, RazonSocial = x.RazonSocial, Correo = x.Correo, Telefono = x.Telefono, Direccion = x.Direccion, Estado = x.Estado };
        public static ClientePerfilResponse ClientePerfil(ClienteDataModel x) => new() { Guid = x.Guid, Nombres = x.Nombres, Apellidos = x.Apellidos, Correo = x.Correo, Telefono = x.Telefono, Direccion = x.Direccion };
        public static ClienteDataModel Cliente(CrearClienteRequest x) => new() { UsuarioId = x.UsuarioId, TipoIdentificacion = x.TipoIdentificacion, NumeroIdentificacion = x.NumeroIdentificacion, Nombres = x.Nombres, Apellidos = x.Apellidos, RazonSocial = x.RazonSocial, Correo = x.Correo, Telefono = x.Telefono, Direccion = x.Direccion, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
        public static ClienteDataModel Cliente(ActualizarClienteRequest x) => new() { Guid = x.Guid, TipoIdentificacion = x.TipoIdentificacion, NumeroIdentificacion = x.NumeroIdentificacion, Nombres = x.Nombres, Apellidos = x.Apellidos, RazonSocial = x.RazonSocial, Correo = x.Correo, Telefono = x.Telefono, Direccion = x.Direccion, Estado = x.Estado };
}

