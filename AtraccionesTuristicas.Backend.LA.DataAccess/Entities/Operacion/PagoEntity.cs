namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

public sealed class PagoEntity
{
    public int pag_id { get; set; }
    public Guid pag_guid { get; set; }
    public int rev_id { get; set; }
    public string? pag_referencia { get; set; }
    public string pag_metodo { get; set; } = string.Empty;
    public decimal pag_monto { get; set; }
    public string pag_moneda { get; set; } = "USD";
    public DateTime pag_fecha_utc { get; set; }
    public string pag_estado { get; set; } = "PENDIENTE";
    public string? pag_origen_canal { get; set; }
    public string pag_usuario_ingreso { get; set; } = string.Empty;
    public string pag_ip_ingreso { get; set; } = string.Empty;
    public DateTime? pag_fecha_mod { get; set; }
    public string? pag_usuario_mod { get; set; }
    public string? pag_ip_mod { get; set; }

    public ReservaEntity? Reserva { get; set; }
}
