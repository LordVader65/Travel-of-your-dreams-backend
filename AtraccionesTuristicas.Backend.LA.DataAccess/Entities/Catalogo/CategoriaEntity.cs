using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

public sealed class CategoriaEntity
{
    public int cat_id { get; set; }
    public Guid cat_guid { get; set; }
    public int? cat_parent_id { get; set; }
    public string cat_nombre { get; set; } = string.Empty;
    public string? cat_tagname { get; set; }
    public DateTime cat_fecha_ingreso { get; set; }
    public string cat_usuario_ingreso { get; set; } = string.Empty;
    public string cat_ip_ingreso { get; set; } = string.Empty;
    public DateTime? cat_fecha_mod { get; set; }
    public string? cat_usuario_mod { get; set; }
    public string? cat_ip_mod { get; set; }
    public DateTime? cat_fecha_eliminacion { get; set; }
    public string? cat_usuario_eliminacion { get; set; }
    public string? cat_ip_eliminacion { get; set; }
    public string cat_estado { get; set; } = "A";

    public CategoriaEntity? Parent { get; set; }
    public ICollection<CategoriaEntity> Children { get; set; } = [];
    public ICollection<CategoriaAtraccionEntity> CategoriaAtracciones { get; set; } = [];
}
