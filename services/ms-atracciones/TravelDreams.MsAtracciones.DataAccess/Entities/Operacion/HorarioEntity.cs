using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;

namespace TravelDreams.MsAtracciones.DataAccess.Entities.Operacion;

public sealed class HorarioEntity
{
    public int hor_id { get; set; }
    public Guid hor_guid { get; set; }
    public int at_id { get; set; }
    public DateOnly hor_fecha { get; set; }
    public TimeOnly hor_hora_inicio { get; set; }
    public TimeOnly? hor_hora_fin { get; set; }
    public int hor_cupos_disponibles { get; set; }
    public string hor_dias_semana { get; set; } = "0,1,2,3,4,5,6";
    public DateTime hor_fecha_ingreso { get; set; }
    public string hor_usuario_ingreso { get; set; } = string.Empty;
    public string hor_ip_ingreso { get; set; } = string.Empty;
    public DateTime? hor_fecha_mod { get; set; }
    public string? hor_usuario_mod { get; set; }
    public string? hor_ip_mod { get; set; }
    public DateTime? hor_fecha_eliminacion { get; set; }
    public string? hor_usuario_eliminacion { get; set; }
    public string? hor_ip_eliminacion { get; set; }
    public string hor_estado { get; set; } = "A";

    public AtraccionEntity? Atraccion { get; set; }
}
