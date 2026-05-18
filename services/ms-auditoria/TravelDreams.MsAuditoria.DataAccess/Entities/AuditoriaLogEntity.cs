namespace TravelDreams.MsAuditoria.DataAccess.Entities;

public sealed class AuditoriaLogEntity
{
    public long log_id { get; set; }
    public Guid log_guid { get; set; }
    public string log_servicio { get; set; } = string.Empty;
    public string log_tabla { get; set; } = string.Empty;
    public string log_operacion { get; set; } = string.Empty;
    public int? log_registro_id { get; set; }
    public Guid? log_registro_guid { get; set; }
    public string? log_datos_anteriores { get; set; }
    public string? log_datos_nuevos { get; set; }
    public DateTime log_fecha_utc { get; set; }
    public string log_usuario { get; set; } = string.Empty;
    public string log_ip { get; set; } = string.Empty;
    public string? log_origen_canal { get; set; }
    public string? log_correlation_id { get; set; }
    public Guid? evento_id { get; set; }
}
