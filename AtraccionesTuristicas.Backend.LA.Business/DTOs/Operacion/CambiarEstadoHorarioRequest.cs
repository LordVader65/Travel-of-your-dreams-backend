namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class CambiarEstadoHorarioRequest
{
    public Guid Guid { get; set; }
    public string Estado { get; set; } = "A";
    public string Usuario { get; set; } = string.Empty;
    public string Ip { get; set; } = "127.0.0.1";
}
