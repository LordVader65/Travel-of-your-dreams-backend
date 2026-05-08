using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

public sealed class TicketEntity
{
    public int tck_id { get; set; }
    public Guid tck_guid { get; set; }
    public int at_id { get; set; }
    public string tck_titulo { get; set; } = string.Empty;
    public decimal tck_precio { get; set; }
    public string tck_moneda { get; set; } = "USD";
    public string tck_tipo_participante { get; set; } = "Adulto";
    public int tck_capacidad_maxima { get; set; }
    public DateTime tck_fecha_ingreso { get; set; }
    public string tck_usuario_ingreso { get; set; } = string.Empty;
    public string tck_ip_ingreso { get; set; } = string.Empty;
    public DateTime? tck_fecha_mod { get; set; }
    public string? tck_usuario_mod { get; set; }
    public string? tck_ip_mod { get; set; }
    public DateTime? tck_fecha_eliminacion { get; set; }
    public string? tck_usuario_eliminacion { get; set; }
    public string? tck_ip_eliminacion { get; set; }
    public string tck_estado { get; set; } = "A";

    public AtraccionEntity? Atraccion { get; set; }
    public ICollection<ReservaDetalleEntity> ReservaDetalles { get; set; } = [];
}
