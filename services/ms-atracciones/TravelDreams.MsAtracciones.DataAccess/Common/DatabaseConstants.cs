namespace TravelDreams.MsAtracciones.DataAccess.Common;

public static class DatabaseConstants
{
    public const string EstadoActivo = "A";
    public const string EstadoInactivo = "I";

    public const string ReservaPendiente = "PENDIENTE";
    public const string ReservaPagada = "PAGADA";
    public const string ReservaConfirmada = "CONFIRMADA";
    public const string ReservaCancelada = "CANCELADA";
    public const string ReservaExpirada = "EXPIRADA";
    public const string ReservaUsada = "USADA";
    public const string ReservaNoShow = "NO_SHOW";

    public const string PagoPendiente = "PENDIENTE";
    public const string PagoAprobado = "APROBADO";
    public const string PagoRechazado = "RECHAZADO";

    public const string MonedaDefault = "USD";

    public const string Incluye = "INCLUYE";
    public const string NoIncluye = "NO_INCLUYE";
    public const string Etiqueta = "ETIQUETA";

    public static class Tables
    {
        public const string Roles = "roles";
        public const string Usuario = "usuario";
        public const string UsuarioRoles = "usuarioxroles";
        public const string Clientes = "clientes";
        public const string DatosFacturacion = "datos_facturacion";
        public const string Destino = "destino";
        public const string Categoria = "categoria";
        public const string Idioma = "idioma";
        public const string Incluye = "incluye";
        public const string Imagen = "imagen";
        public const string Atraccion = "atraccion";
        public const string CategoriaAtraccion = "categoria_atraccion";
        public const string IdiomaAtraccion = "idioma_atraccion";
        public const string ImagenAtraccion = "imagen_atraccion";
        public const string AtraccionIncluye = "atraccion_incluye";
        public const string Ticket = "ticket";
        public const string Horario = "horario";
        public const string HorarioRegla = "horario_regla";
        public const string Reservas = "reservas";
        public const string ReservaDetalle = "reserva_detalle";
        public const string ReservaEstadoHistorial = "reserva_estado_historial";
        public const string Pagos = "pagos";
        public const string Facturas = "facturas";
        public const string Resenia = "resenia";
        public const string AuditoriaLog = "auditoria_log";
    }
}
