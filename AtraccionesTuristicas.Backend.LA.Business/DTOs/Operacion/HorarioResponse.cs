namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Operacion;

public class HorarioResponse { public int Id { get; set; } public Guid Guid { get; set; } public int AtraccionId { get; set; } public DateOnly Fecha { get; set; } public TimeOnly HoraInicio { get; set; } public TimeOnly? HoraFin { get; set; } public int CuposDisponibles { get; set; } public string Estado { get; set; } = "A"; }

