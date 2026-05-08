namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Cliente;

public sealed class ClientePerfilResponse { public Guid Guid { get; set; } public string? Nombres { get; set; } public string? Apellidos { get; set; } public string Correo { get; set; } = string.Empty; public string? Telefono { get; set; } public string? Direccion { get; set; } }

