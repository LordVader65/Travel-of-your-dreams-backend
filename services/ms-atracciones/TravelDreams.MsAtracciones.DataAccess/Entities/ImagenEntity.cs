using TravelDreams.MsAtracciones.DataAccess.Entities.CatalogoRelaciones;

namespace TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;

public sealed class ImagenEntity
{
    public int img_id { get; set; }
    public Guid img_guid { get; set; }
    public string img_url { get; set; } = string.Empty;
    public string? img_descripcion { get; set; }
    public DateTime img_fecha_ingreso { get; set; }
    public string img_usuario_ingreso { get; set; } = string.Empty;
    public string img_ip_ingreso { get; set; } = string.Empty;
    public DateTime? img_fecha_mod { get; set; }
    public string? img_usuario_mod { get; set; }
    public string? img_ip_mod { get; set; }
    public DateTime? img_fecha_eliminacion { get; set; }
    public string? img_usuario_eliminacion { get; set; }
    public string? img_ip_eliminacion { get; set; }
    public string img_estado { get; set; } = "A";

    public ICollection<ImagenAtraccionEntity> ImagenAtracciones { get; set; } = [];
}
