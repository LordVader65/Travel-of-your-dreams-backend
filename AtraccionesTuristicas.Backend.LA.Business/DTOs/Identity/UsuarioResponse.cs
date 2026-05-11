namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Identity;

public sealed class UsuarioResponse
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Estado { get; set; } = "A";
    public IReadOnlyList<string> Roles { get; set; } = [];
    public int? ClienteId { get; set; }
    public Guid? ClienteGuid { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteIdentificacion { get; set; }
    public string? ClienteCorreo { get; set; }
}
