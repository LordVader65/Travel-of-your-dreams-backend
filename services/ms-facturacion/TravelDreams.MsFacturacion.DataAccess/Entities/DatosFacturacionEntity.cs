namespace TravelDreams.MsFacturacion.DataAccess.Entities;

public sealed class DatosFacturacionEntity
{
    public int dfac_id { get; set; }
    public Guid dfac_guid { get; set; }
    public Guid cli_guid { get; set; }
    public string dfac_tipo_identificacion { get; set; } = string.Empty;
    public string dfac_numero_identificacion { get; set; } = string.Empty;
    public string? dfac_razon_social { get; set; }
    public string dfac_nombre { get; set; } = string.Empty;
    public string? dfac_apellido { get; set; }
    public string dfac_correo { get; set; } = string.Empty;
    public string? dfac_telefono { get; set; }
    public string? dfac_direccion { get; set; }
    public DateTime dfac_fecha_ingreso { get; set; }
    public string dfac_usuario_ingreso { get; set; } = string.Empty;
    public string dfac_ip_ingreso { get; set; } = string.Empty;
    public DateTime? dfac_fecha_mod { get; set; }
    public string? dfac_usuario_mod { get; set; }
    public string? dfac_ip_mod { get; set; }
    public DateTime? dfac_fecha_eliminacion { get; set; }
    public string? dfac_usuario_eliminacion { get; set; }
    public string? dfac_ip_eliminacion { get; set; }
    public string dfac_estado { get; set; } = "A";
    public long dfac_row_version { get; set; } = 1;

    public ICollection<PagoEntity> Pagos { get; set; } = [];
    public ICollection<FacturaEntity> Facturas { get; set; } = [];
}
