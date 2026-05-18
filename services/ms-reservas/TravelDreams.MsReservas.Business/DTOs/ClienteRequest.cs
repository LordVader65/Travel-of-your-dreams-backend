namespace TravelDreams.MsReservas.Business.DTOs;

public sealed class ClienteRequest
{
    public Guid? ClienteGuid { get; set; }
    public Guid? UsuarioGuid { get; set; }
    public string TipoIdentificacion { get; set; } = "OTRO";
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? RazonSocial { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
