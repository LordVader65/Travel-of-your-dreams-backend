namespace TravelDreams.MsIdentidad.Business.DTOs;

public sealed class UsuarioResponse
{
    public Guid Guid { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Estado { get; set; } = "A";
    public IReadOnlyList<RolResponse> Roles { get; set; } = [];
}

public sealed class RolResponse
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "A";
}
