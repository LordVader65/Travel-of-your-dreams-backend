namespace TravelDreams.MsIdentidad.Business.DTOs;

public sealed class CambiarPasswordRequest
{
    public string? PasswordActual { get; set; }
    public string PasswordNueva { get; set; } = string.Empty;
}
