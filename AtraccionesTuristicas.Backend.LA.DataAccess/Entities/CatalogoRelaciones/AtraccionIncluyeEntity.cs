using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

public sealed class AtraccionIncluyeEntity
{
    public int inc_id { get; set; }
    public int at_id { get; set; }
    public DateTime ai_fecha_ingreso { get; set; }
    public string ai_usuario_ingreso { get; set; } = string.Empty;
    public DateTime? ai_fecha_eliminacion { get; set; }
    public string? ai_usuario_eliminacion { get; set; }
    public string ai_estado { get; set; } = "A";

    public IncluyeEntity? Incluye { get; set; }
    public AtraccionEntity? Atraccion { get; set; }
}
