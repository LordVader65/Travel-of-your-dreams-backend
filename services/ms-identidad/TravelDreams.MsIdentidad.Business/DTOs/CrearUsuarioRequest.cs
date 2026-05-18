namespace TravelDreams.MsIdentidad.Business.DTOs;

public sealed class CrearUsuarioRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IReadOnlyList<int> RolIds { get; set; } = [];
}
