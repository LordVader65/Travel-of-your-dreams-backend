using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

public sealed class ReseniaEntity
{
    public int rsn_id { get; set; }
    public Guid rsn_guid { get; set; }
    public int at_id { get; set; }
    public int rev_id { get; set; }
    public string? rsn_comentario { get; set; }
    public short rsn_rating { get; set; }
    public DateTime rsn_fecha_creacion { get; set; }
    public string rsn_usuario_creacion { get; set; } = string.Empty;
    public string rsn_ip_creacion { get; set; } = string.Empty;
    public DateTime? rsn_fecha_mod { get; set; }
    public string? rsn_usuario_mod { get; set; }
    public string? rsn_ip_mod { get; set; }
    public DateTime? rsn_fecha_eliminacion { get; set; }
    public string? rsn_usuario_eliminacion { get; set; }
    public string? rsn_ip_eliminacion { get; set; }
    public string rsn_estado { get; set; } = "A";

    public AtraccionEntity? Atraccion { get; set; }
    public ReservaEntity? Reserva { get; set; }
}
