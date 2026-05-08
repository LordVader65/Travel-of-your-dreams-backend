namespace AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Identity;

public sealed class UsuarioRolEntity
{
    public int usu_rol_id { get; set; }
    public int usu_id { get; set; }
    public int rol_id { get; set; }
    public string usu_rol_estado { get; set; } = "A";

    public UsuarioEntity? Usuario { get; set; }
    public RolEntity? Rol { get; set; }
}
