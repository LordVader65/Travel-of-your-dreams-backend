using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Catalogo;

public sealed class AtraccionEntity
{
    public int at_id { get; set; }
    public Guid at_guid { get; set; }
    public int des_id { get; set; }
    public string? at_num_establecimiento { get; set; }
    public string at_nombre { get; set; } = string.Empty;
    public string? at_descripcion { get; set; }
    public int at_total_resenias { get; set; }
    public string? at_direccion { get; set; }
    public int? at_duracion_minutos { get; set; }
    public string? at_punto_encuentro { get; set; }
    public decimal? at_precio_referencia { get; set; }
    public bool at_incluye_acompaniante { get; set; }
    public bool at_incluye_transporte { get; set; }
    public bool at_disponible { get; set; } = true;
    public bool at_free_cancellation { get; set; }
    public bool at_skip_the_line { get; set; }
    public DateTime at_fecha_ingreso { get; set; }
    public string at_usuario_ingreso { get; set; } = string.Empty;
    public string at_ip_ingreso { get; set; } = string.Empty;
    public DateTime? at_fecha_mod { get; set; }
    public string? at_usuario_mod { get; set; }
    public string? at_ip_mod { get; set; }
    public DateTime? at_fecha_eliminacion { get; set; }
    public string? at_usuario_eliminacion { get; set; }
    public string? at_ip_eliminacion { get; set; }
    public string at_estado { get; set; } = "A";

    public DestinoEntity? Destino { get; set; }
    public ICollection<CategoriaAtraccionEntity> CategoriaAtracciones { get; set; } = [];
    public ICollection<IdiomaAtraccionEntity> IdiomaAtracciones { get; set; } = [];
    public ICollection<ImagenAtraccionEntity> ImagenAtracciones { get; set; } = [];
    public ICollection<AtraccionIncluyeEntity> AtraccionIncluyes { get; set; } = [];
    public ICollection<TicketEntity> Tickets { get; set; } = [];
    public ICollection<HorarioEntity> Horarios { get; set; } = [];
    public ICollection<ReseniaEntity> Resenias { get; set; } = [];
}
