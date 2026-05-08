using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

public sealed class IdiomaAtraccionEntity
{
    public int id_id { get; set; }
    public int at_id { get; set; }
    public DateTime ia_fecha_ingreso { get; set; }
    public string ia_usuario_ingreso { get; set; } = string.Empty;
    public DateTime? ia_fecha_eliminacion { get; set; }
    public string? ia_usuario_eliminacion { get; set; }
    public string ia_estado { get; set; } = "A";

    public IdiomaEntity? Idioma { get; set; }
    public AtraccionEntity? Atraccion { get; set; }
}
