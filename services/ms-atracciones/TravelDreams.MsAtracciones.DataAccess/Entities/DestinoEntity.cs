namespace TravelDreams.MsAtracciones.DataAccess.Entities.Catalogo;

public sealed class DestinoEntity
{
    public int des_id { get; set; }
    public Guid des_guid { get; set; }
    public string des_nombre { get; set; } = string.Empty;
    public string des_pais { get; set; } = string.Empty;
    public string? des_imagen_url { get; set; }
    public DateTime des_fecha_ingreso { get; set; }
    public string des_usuario_ingreso { get; set; } = string.Empty;
    public string des_ip_ingreso { get; set; } = string.Empty;
    public DateTime? des_fecha_mod { get; set; }
    public string? des_usuario_mod { get; set; }
    public string? des_ip_mod { get; set; }
    public DateTime? des_fecha_eliminacion { get; set; }
    public string? des_usuario_eliminacion { get; set; }
    public string? des_ip_eliminacion { get; set; }
    public string des_estado { get; set; } = "A";

    public ICollection<AtraccionEntity> Atracciones { get; set; } = [];
}
