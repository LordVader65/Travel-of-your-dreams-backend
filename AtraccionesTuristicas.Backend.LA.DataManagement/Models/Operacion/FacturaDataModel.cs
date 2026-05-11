namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

public sealed class FacturaDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int ReservaId { get; set; }
    public int? PagoId { get; set; }
    public int? DatosFacturacionId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Total { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observacion { get; set; }
    public string Estado { get; set; } = "A";
    public FacturaReservaDataModel? Reserva { get; set; }
    public FacturaClienteDataModel? Cliente { get; set; }
    public FacturaDatosFacturacionDataModel? DatosFacturacion { get; set; }
    public FacturaAtraccionDataModel? Atraccion { get; set; }
    public FacturaPagoDataModel? Pago { get; set; }
    public IReadOnlyList<FacturaDetalleDataModel> Detalles { get; set; } = [];
}

public sealed class FacturaReservaDataModel { public int Id { get; set; } public Guid Guid { get; set; } public string Codigo { get; set; } = string.Empty; public DateTime FechaReservaUtc { get; set; } public DateTime FechaExpiracionUtc { get; set; } public string Estado { get; set; } = string.Empty; }
public sealed class FacturaClienteDataModel { public int Id { get; set; } public Guid Guid { get; set; } public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? Nombres { get; set; } public string? Apellidos { get; set; } public string? RazonSocial { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } }
public sealed class FacturaDatosFacturacionDataModel { public int Id { get; set; } public Guid Guid { get; set; } public string TipoIdentificacion { get; set; } = string.Empty; public string NumeroIdentificacion { get; set; } = string.Empty; public string? RazonSocial { get; set; } public string Nombre { get; set; } = string.Empty; public string? Apellido { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } }
public sealed class FacturaAtraccionDataModel { public int Id { get; set; } public Guid Guid { get; set; } public string Nombre { get; set; } = string.Empty; public string? Direccion { get; set; } public DateOnly Fecha { get; set; } public TimeOnly HoraInicio { get; set; } public TimeOnly? HoraFin { get; set; } public string? Destino { get; set; } public string? Pais { get; set; } }
public sealed class FacturaPagoDataModel { public int Id { get; set; } public Guid Guid { get; set; } public string? Referencia { get; set; } public string Metodo { get; set; } = string.Empty; public DateTime FechaUtc { get; set; } public string Estado { get; set; } = string.Empty; }
public sealed class FacturaDetalleDataModel { public int Id { get; set; } public string TicketTitulo { get; set; } = string.Empty; public string TipoParticipante { get; set; } = string.Empty; public int Cantidad { get; set; } public decimal PrecioUnitario { get; set; } public decimal Subtotal { get; set; } }
