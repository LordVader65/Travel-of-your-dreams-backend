namespace TravelDreams.MsReservas.DataAccess.Entities;

public sealed class ReservaEstadoHistorialEntity
{
    public int reh_id { get; set; }
    public Guid reh_guid { get; set; }
    public int rev_id { get; set; }
    public string? reh_estado_anterior { get; set; }
    public string reh_estado_nuevo { get; set; } = string.Empty;
    public DateTime reh_fecha_utc { get; set; }
    public string reh_usuario { get; set; } = string.Empty;
    public string reh_ip { get; set; } = string.Empty;
    public string? reh_observacion { get; set; }

    public ReservaEntity? Reserva { get; set; }
}
