using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Auditoria;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IAuditoriaLogRepository : IRepositoryBase<AuditoriaLogEntity>
{
    Task<IReadOnlyList<AuditoriaLogEntity>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditoriaLogEntity>> ConsultarAsync(string? tabla, string? operacion, string? usuario, DateTime? desdeUtc, DateTime? hastaUtc, int page, int limit, CancellationToken cancellationToken = default);
    Task<int> ContarAsync(string? tabla, string? operacion, string? usuario, DateTime? desdeUtc, DateTime? hastaUtc, CancellationToken cancellationToken = default);
    Task<long> RegistrarAuditoriaAsync(
        string tabla,
        string operacion,
        int? registroId,
        Guid? registroGuid,
        string? datosAnteriores,
        string? datosNuevos,
        string usuario,
        string ip,
        string? origenCanal = null,
        CancellationToken cancellationToken = default);
}
