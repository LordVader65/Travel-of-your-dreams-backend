namespace TravelDreams.MsIdentidad.DataManagement.Models;

public sealed class RolDataModel
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "A";
}
