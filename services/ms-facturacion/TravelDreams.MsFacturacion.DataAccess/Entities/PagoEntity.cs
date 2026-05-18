namespace TravelDreams.MsFacturacion.DataAccess.Entities;

public sealed class PagoEntity
{
    public int pag_id { get; set; }
    public Guid pag_guid { get; set; }
    public Guid rev_guid { get; set; }
    public Guid cli_guid { get; set; }
    public int? dfac_id { get; set; }
    public decimal pag_monto { get; set; }
    public string pag_moneda { get; set; } = "USD";
    public string pag_metodo { get; set; } = string.Empty;
    public string pag_referencia { get; set; } = string.Empty;
    public DateTime pag_fecha_utc { get; set; }
    public string? pag_origen_canal { get; set; }
    public string pag_estado { get; set; } = "APROBADO";
    public string pag_usuario_ingreso { get; set; } = string.Empty;
    public string pag_ip_ingreso { get; set; } = string.Empty;
    public string? pag_observacion { get; set; }
    public long pag_row_version { get; set; } = 1;

    public DatosFacturacionEntity? DatosFacturacion { get; set; }
    public FacturaEntity? Factura { get; set; }
}
