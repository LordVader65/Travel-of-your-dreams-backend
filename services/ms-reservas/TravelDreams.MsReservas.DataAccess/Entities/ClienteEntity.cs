namespace TravelDreams.MsReservas.DataAccess.Entities;

public sealed class ClienteEntity
{
    public int cli_id { get; set; }
    public Guid cli_guid { get; set; }
    public Guid? usu_guid { get; set; }
    public string cli_tipo_identificacion { get; set; } = string.Empty;
    public string cli_numero_identificacion { get; set; } = string.Empty;
    public string? cli_nombres { get; set; }
    public string? cli_apellidos { get; set; }
    public string? cli_razon_social { get; set; }
    public string cli_correo { get; set; } = string.Empty;
    public string? cli_telefono { get; set; }
    public string? cli_direccion { get; set; }
    public DateTime cli_fecha_ingreso { get; set; }
    public string cli_usuario_ingreso { get; set; } = string.Empty;
    public string cli_ip_ingreso { get; set; } = string.Empty;
    public DateTime? cli_fecha_eliminacion { get; set; }
    public string? cli_usuario_eliminacion { get; set; }
    public string? cli_ip_eliminacion { get; set; }
    public string cli_estado { get; set; } = "A";
    public long cli_row_version { get; set; } = 1;

    public ICollection<ReservaEntity> Reservas { get; set; } = [];
}
