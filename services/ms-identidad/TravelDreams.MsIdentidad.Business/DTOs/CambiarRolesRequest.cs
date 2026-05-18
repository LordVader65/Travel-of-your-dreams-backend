namespace TravelDreams.MsIdentidad.Business.DTOs;

public sealed class CambiarRolesRequest
{
    public IReadOnlyList<int> RolIds { get; set; } = [];
}
