namespace TravelDreams.MsFacturacion.DataAccess.Entities;

public sealed class FacturaEntity
{
    public int fac_id { get; set; }
    public Guid fac_guid { get; set; }
    public string fac_numero { get; set; } = string.Empty;
    public Guid rev_guid { get; set; }
    public Guid cli_guid { get; set; }
    public int pag_id { get; set; }
    public int? dfac_id { get; set; }
    public DateTime fac_fecha_emision_utc { get; set; }
    public decimal fac_subtotal { get; set; }
    public decimal fac_valor_iva { get; set; }
    public decimal fac_total { get; set; }
    public string fac_moneda { get; set; } = "USD";
    public string? fac_observacion { get; set; }
    public string fac_usuario_ingreso { get; set; } = string.Empty;
    public string fac_ip_ingreso { get; set; } = string.Empty;
    public DateTime? fac_fecha_eliminacion { get; set; }
    public string? fac_usuario_eliminacion { get; set; }
    public string? fac_ip_eliminacion { get; set; }
    public string fac_estado { get; set; } = "A";
    public long fac_row_version { get; set; } = 1;

    public PagoEntity? Pago { get; set; }
    public DatosFacturacionEntity? DatosFacturacion { get; set; }
}
