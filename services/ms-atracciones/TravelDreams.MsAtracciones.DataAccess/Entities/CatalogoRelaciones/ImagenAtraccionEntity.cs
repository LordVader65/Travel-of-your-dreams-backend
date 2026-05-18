using TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;

namespace TravelDreams.MsAtracciones.DataAccess.Entities.CatalogoRelaciones;

public sealed class ImagenAtraccionEntity
{
    public int img_id { get; set; }
    public int at_id { get; set; }
    public bool ima_es_principal { get; set; }
    public int ima_orden { get; set; }
    public DateTime ima_fecha_ingreso { get; set; }
    public string ima_usuario_ingreso { get; set; } = string.Empty;
    public DateTime? ima_fecha_eliminacion { get; set; }
    public string? ima_usuario_eliminacion { get; set; }
    public string ima_estado { get; set; } = "A";

    public ImagenEntity? Imagen { get; set; }
    public AtraccionEntity? Atraccion { get; set; }
}
