using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Auditoria;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Auditoria;

public interface IAuditoriaLogDataService
{
    Task<IReadOnlyList<AuditoriaLogDataModel>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default);
    Task<long> RegistrarAsync(AuditoriaLogDataModel model, CancellationToken cancellationToken = default);
}
