namespace TravelDreams.MsIdentidad.DataAccess.Entities;

public sealed class UsuarioEntity
{
    public int usu_id { get; set; }
    public Guid usu_guid { get; set; }
    public string usu_login { get; set; } = string.Empty;
    public string usu_password_hash { get; set; } = string.Empty;
    public DateTime usu_fecha_registro { get; set; }
    public string usu_usuario_registro { get; set; } = string.Empty;
    public string usu_ip_registro { get; set; } = string.Empty;
    public DateTime? usu_fecha_mod { get; set; }
    public string? usu_usuario_mod { get; set; }
    public string? usu_ip_mod { get; set; }
    public DateTime? usu_fecha_eliminacion { get; set; }
    public string? usu_usuario_eliminacion { get; set; }
    public string? usu_ip_eliminacion { get; set; }
    public string usu_estado { get; set; } = "A";

    public ICollection<UsuarioRolEntity> UsuarioRoles { get; set; } = [];
}
