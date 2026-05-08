namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public sealed class ActualizarTicketRequest : CrearTicketRequest { public Guid Guid { get; set; } public string Estado { get; set; } = "A"; }

