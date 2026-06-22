namespace TravelDreams.MsFacturacion.DataAccess.Entities.V3;

public sealed class MarketplacePagoSolicitudV3Entity
{
    public long fsol_id { get; set; }
    public Guid fsol_correlation_id { get; set; }
    public string fsol_estado { get; set; } = "PROCESANDO";
    public Guid cli_guid { get; set; }
    public Guid rev_guid { get; set; }
    public Guid? fac_guid { get; set; }
    public string? fac_numero { get; set; }
    public string? fsol_error { get; set; }
    public string fsol_payload_json { get; set; } = "{}";
    public DateTime fsol_created_at_utc { get; set; }
    public DateTime fsol_updated_at_utc { get; set; }
}
