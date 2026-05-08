namespace AtraccionesTuristicas.Backend.LA.Business.Services.Auditoria
{
    using AtraccionesTuristicas.Backend.LA.Business.DTOs.Auditoria;
    using AtraccionesTuristicas.Backend.LA.Business.Interfaces.Auditoria;
    using AtraccionesTuristicas.Backend.LA.Business.Mappers;

    public sealed class AuditoriaLogService : IAuditoriaLogService
    {
        private readonly IAuditoriaLogDataService _data;
        public AuditoriaLogService(IAuditoriaLogDataService data) => _data = data;
        public async Task<IReadOnlyList<AuditoriaLogResponse>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default) => (await _data.ConsultarPorTablaAsync(tabla, cancellationToken)).Select(Map.Auditoria).ToList();
    }
}

