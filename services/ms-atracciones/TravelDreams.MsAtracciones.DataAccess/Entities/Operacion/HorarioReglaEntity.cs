using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;

namespace TravelDreams.MsAtracciones.DataAccess.Entities.Operacion;

public sealed class HorarioReglaEntity
{
    public int hreg_id { get; set; }
    public Guid hreg_guid { get; set; }
    public int at_id { get; set; }
    public TimeOnly hreg_hora_inicio { get; set; }
    public TimeOnly? hreg_hora_fin { get; set; }
    public string hreg_dias_semana { get; set; } = "0,1,2,3,4,5,6";
    public int hreg_cupos { get; set; }
    public DateOnly hreg_fecha_inicio { get; set; }
    public DateOnly hreg_fecha_fin { get; set; }
    public DateTime hreg_fecha_ingreso { get; set; }
    public string hreg_usuario_ingreso { get; set; } = string.Empty;
    public string hreg_ip_ingreso { get; set; } = string.Empty;
    public DateTime? hreg_fecha_mod { get; set; }
    public string? hreg_usuario_mod { get; set; }
    public string? hreg_ip_mod { get; set; }
    public DateTime? hreg_fecha_eliminacion { get; set; }
    public string? hreg_usuario_eliminacion { get; set; }
    public string? hreg_ip_eliminacion { get; set; }
    public string hreg_estado { get; set; } = "A";

    public AtraccionEntity? Atraccion { get; set; }
}
