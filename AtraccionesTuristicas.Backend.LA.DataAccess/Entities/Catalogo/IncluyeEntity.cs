using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

public sealed class IncluyeEntity
{
    public int inc_id { get; set; }
    public Guid inc_guid { get; set; }
    public string inc_descripcion { get; set; } = string.Empty;
    public string inc_tipo { get; set; } = "INCLUYE";
    public string inc_estado { get; set; } = "A";

    public ICollection<AtraccionIncluyeEntity> AtraccionIncluyes { get; set; } = [];
}
