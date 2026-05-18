namespace TravelDreams.MsIdentidad.Business.DTOs;

public sealed class RegisterRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
