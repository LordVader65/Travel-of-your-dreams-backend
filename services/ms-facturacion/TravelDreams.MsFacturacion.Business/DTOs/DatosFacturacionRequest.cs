namespace TravelDreams.MsFacturacion.Business.DTOs;

public class DatosFacturacionRequest
{
    public Guid? Guid { get; set; }
    public Guid ClienteGuid { get; set; }
    public string TipoIdentificacion { get; set; } = "OTRO";
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string? RazonSocial { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Apellido { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
