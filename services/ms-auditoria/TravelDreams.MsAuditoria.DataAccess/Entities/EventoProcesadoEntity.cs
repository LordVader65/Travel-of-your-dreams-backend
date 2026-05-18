namespace TravelDreams.MsAuditoria.DataAccess.Entities;

public sealed class EventoProcesadoEntity
{
    public long ep_id { get; set; }
    public Guid ep_evento_id { get; set; }
    public string ep_tipo { get; set; } = string.Empty;
    public string ep_origen_servicio { get; set; } = string.Empty;
    public DateTime ep_fecha_procesado_utc { get; set; }
    public string? ep_correlation_id { get; set; }
}
