using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;

public sealed class RolEntity
{
    public int rol_id { get; set; }
    public Guid rol_guid { get; set; }
    public string rol_descripcion { get; set; } = string.Empty;
    public DateTime rol_fecha_ingreso { get; set; }
    public string rol_usuario_ingreso { get; set; } = string.Empty;
    public string rol_ip_ingreso { get; set; } = string.Empty;
    public DateTime? rol_fecha_eliminacion { get; set; }
    public string? rol_usuario_eliminacion { get; set; }
    public string? rol_ip_eliminacion { get; set; }
    public string rol_estado { get; set; } = "A";

    public ICollection<UsuarioRolEntity> UsuarioRoles { get; set; } = [];
}
