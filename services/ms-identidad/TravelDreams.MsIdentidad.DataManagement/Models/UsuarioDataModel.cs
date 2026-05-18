namespace TravelDreams.MsIdentidad.DataManagement.Models;

public sealed class UsuarioDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Estado { get; set; } = "A";
    public IReadOnlyList<RolDataModel> Roles { get; set; } = [];
}
