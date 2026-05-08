using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

public sealed class IdiomaEntity
{
    public int id_id { get; set; }
    public Guid id_guid { get; set; }
    public string id_codigo { get; set; } = string.Empty;
    public string id_descripcion { get; set; } = string.Empty;
    public DateTime id_fecha_ingreso { get; set; }
    public string id_usuario_ingreso { get; set; } = string.Empty;
    public string id_ip_ingreso { get; set; } = string.Empty;
    public DateTime? id_fecha_mod { get; set; }
    public string? id_usuario_mod { get; set; }
    public string? id_ip_mod { get; set; }
    public DateTime? id_fecha_eliminacion { get; set; }
    public string? id_usuario_eliminacion { get; set; }
    public string? id_ip_eliminacion { get; set; }
    public string id_estado { get; set; } = "A";

    public ICollection<IdiomaAtraccionEntity> IdiomaAtracciones { get; set; } = [];
}
