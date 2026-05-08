namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;

public sealed class CambiarPasswordRequest
{
    public Guid UsuarioGuid { get; set; }
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNueva { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Ip { get; set; } = "127.0.0.1";
}
