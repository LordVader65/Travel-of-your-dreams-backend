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
        public async Task<BusinessPagedResult<AuditoriaLogResponse>> ConsultarAsync(AuditoriaLogFiltroRequest filtro, CancellationToken cancellationToken = default)
        {
            var result = await _data.ConsultarAsync(filtro.Tabla, filtro.Operacion, filtro.Usuario, filtro.DesdeUtc, filtro.HastaUtc, filtro.Page, filtro.Limit, cancellationToken);
            return new BusinessPagedResult<AuditoriaLogResponse>
            {
                Items = result.Items.Select(Map.Auditoria).ToList(),
                Page = result.Page,
                Limit = result.Limit,
                Total = result.Total
            };
        }
    }
}

