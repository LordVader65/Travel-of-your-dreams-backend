using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
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

    public async Task<DataPagedResult<AuditoriaLogDataModel>> ConsultarAsync(string? tabla, string? operacion, string? usuario, DateTime? desdeUtc, DateTime? hastaUtc, int page, int limit, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        limit = Math.Clamp(limit, 1, 100);
        var entities = await _unitOfWork.AuditoriaLogs.ConsultarAsync(tabla, operacion, usuario, desdeUtc, hastaUtc, page, limit, cancellationToken);
        var total = await _unitOfWork.AuditoriaLogs.ContarAsync(tabla, operacion, usuario, desdeUtc, hastaUtc, cancellationToken);
        return new DataPagedResult<AuditoriaLogDataModel>
        {
            Items = entities.Select(AuditoriaLogDataMapper.ToDataModel).ToList(),
            Page = page,
            Limit = limit,
            Total = total
        };
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
