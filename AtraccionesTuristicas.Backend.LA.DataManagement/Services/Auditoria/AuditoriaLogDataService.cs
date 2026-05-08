using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Auditoria;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Auditoria;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Auditoria;

public sealed class AuditoriaLogDataService : IAuditoriaLogDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditoriaLogDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<AuditoriaLogDataModel>> ConsultarPorTablaAsync(string tabla, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.AuditoriaLogs.ConsultarPorTablaAsync(tabla, cancellationToken);
        return entities.Select(AuditoriaLogDataMapper.ToDataModel).ToList();
    }

    public Task<long> RegistrarAsync(AuditoriaLogDataModel model, CancellationToken cancellationToken = default) =>
        _unitOfWork.AuditoriaLogs.RegistrarAuditoriaAsync(
            model.Tabla,
            model.Operacion,
            model.RegistroId,
            model.RegistroGuid,
            model.DatosAnteriores,
            model.DatosNuevos,
            model.Usuario,
            model.Ip,
            model.OrigenCanal,
            cancellationToken);
}
