namespace AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

public sealed class UsuarioRolDataModel
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int RolId { get; set; }
    public string Estado { get; set; } = "A";
}
