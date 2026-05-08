using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

public sealed class CategoriaAtraccionEntity
{
    public int cat_id { get; set; }
    public int at_id { get; set; }
    public DateTime ca_fecha_ingreso { get; set; }
    public string ca_usuario_ingreso { get; set; } = string.Empty;
    public DateTime? ca_fecha_eliminacion { get; set; }
    public string? ca_usuario_eliminacion { get; set; }
    public string ca_estado { get; set; } = "A";

    public CategoriaEntity? Categoria { get; set; }
    public AtraccionEntity? Atraccion { get; set; }
}
