namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auditoria
{
    using AtraccionesTuristicas.Backend.LA.Business.DTOs.Auditoria;
    public interface IAuditoriaLogService { Task<IReadOnlyList<AuditoriaLogResponse>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default); }
}

