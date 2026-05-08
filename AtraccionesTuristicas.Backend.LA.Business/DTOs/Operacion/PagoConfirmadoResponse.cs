namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class PagoConfirmadoResponse
{
    public Guid PagoGuid { get; set; }
    public Guid? FacturaGuid { get; set; }
}
