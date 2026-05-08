using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

public sealed class ReservaEntity
{
    public int rev_id { get; set; }
    public Guid rev_guid { get; set; }
    public string rev_codigo { get; set; } = string.Empty;
    public int cli_id { get; set; }
    public int hor_id { get; set; }
    public DateTime rev_fecha_reserva_utc { get; set; }
    public DateTime rev_fecha_expiracion_utc { get; set; }
    public decimal rev_subtotal { get; set; }
    public decimal rev_valor_iva { get; set; }
    public decimal rev_total { get; set; }
    public string rev_moneda { get; set; } = "USD";
    public string? rev_origen_canal { get; set; }
    public string rev_usuario_ingreso { get; set; } = string.Empty;
    public string rev_ip_ingreso { get; set; } = string.Empty;
    public DateTime? rev_fecha_mod { get; set; }
    public string? rev_usuario_mod { get; set; }
    public string? rev_ip_mod { get; set; }
    public DateTime? rev_fecha_cancelacion { get; set; }
    public string? rev_usuario_cancelacion { get; set; }
    public string? rev_ip_cancelacion { get; set; }
    public string? rev_motivo_cancelacion { get; set; }
    public string rev_estado { get; set; } = "PENDIENTE";

    public ClienteEntity? Cliente { get; set; }
    public HorarioEntity? Horario { get; set; }
    public ICollection<ReservaDetalleEntity> Detalles { get; set; } = [];
    public ICollection<ReservaEstadoHistorialEntity> EstadoHistorial { get; set; } = [];
    public ICollection<PagoEntity> Pagos { get; set; } = [];
    public FacturaEntity? Factura { get; set; }
    public ReseniaEntity? Resenia { get; set; }
}
