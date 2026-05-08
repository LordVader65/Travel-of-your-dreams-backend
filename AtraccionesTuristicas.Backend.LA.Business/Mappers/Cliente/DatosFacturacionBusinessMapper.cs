namespace AtraccionesTuristicas.Backend.LA.Business.Mappers;

internal static partial class Map
{
        public static DatosFacturacionResponse Datos(DatosFacturacionDataModel x) => new() { Id = x.Id, Guid = x.Guid, ClienteId = x.ClienteId, TipoIdentificacion = x.TipoIdentificacion, NumeroIdentificacion = x.NumeroIdentificacion, RazonSocial = x.RazonSocial, Nombre = x.Nombre, Apellido = x.Apellido, Correo = x.Correo, Telefono = x.Telefono, Direccion = x.Direccion, Estado = x.Estado };
        public static DatosFacturacionDataModel Datos(CrearDatosFacturacionRequest x) => new() { ClienteId = x.ClienteId, TipoIdentificacion = x.TipoIdentificacion, NumeroIdentificacion = x.NumeroIdentificacion, RazonSocial = x.RazonSocial, Nombre = x.Nombre, Apellido = x.Apellido, Correo = x.Correo, Telefono = x.Telefono, Direccion = x.Direccion, UsuarioIngreso = x.UsuarioIngreso, IpIngreso = x.IpIngreso, Estado = BusinessConstants.EstadoActivo };
        public static DatosFacturacionDataModel Datos(ActualizarDatosFacturacionRequest x) { var m = Datos((CrearDatosFacturacionRequest)x); m.Guid = x.Guid; m.Estado = x.Estado; return m; }
}

