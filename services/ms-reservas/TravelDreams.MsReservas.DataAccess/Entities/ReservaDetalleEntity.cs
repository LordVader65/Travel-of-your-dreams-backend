namespace TravelDreams.MsReservas.DataAccess.Entities;

public sealed class ReservaDetalleEntity
{
    public int rdet_id { get; set; }
    public Guid rdet_guid { get; set; }
    public int rev_id { get; set; }
    public Guid tck_guid { get; set; }
    public string rdet_ticket_titulo { get; set; } = string.Empty;
    public string rdet_tipo_participante { get; set; } = string.Empty;
    public int rdet_cantidad { get; set; }
    public decimal rdet_precio_unit { get; set; }
    public decimal rdet_subtotal { get; set; }
    public DateTime rdet_fecha_ingreso { get; set; }
    public string rdet_usuario_ingreso { get; set; } = string.Empty;
    public string rdet_ip_ingreso { get; set; } = string.Empty;
    public string rdet_estado { get; set; } = "A";

    public ReservaEntity? Reserva { get; set; }
}
