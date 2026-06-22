namespace TravelDreams.MsReservas.DataAccess.Entities.V3;

public sealed class MarketplaceReservaSolicitudV3Entity
{
    public long rsol_id { get; set; }
    public Guid rsol_correlation_id { get; set; }
    public string rsol_estado { get; set; } = "PROCESANDO";
    public Guid cli_guid { get; set; }
    public Guid? rev_guid { get; set; }
    public string? rev_codigo { get; set; }
    public string? rsol_error { get; set; }
    public string rsol_payload_json { get; set; } = "{}";
    public DateTime rsol_created_at_utc { get; set; }
    public DateTime rsol_updated_at_utc { get; set; }
}
