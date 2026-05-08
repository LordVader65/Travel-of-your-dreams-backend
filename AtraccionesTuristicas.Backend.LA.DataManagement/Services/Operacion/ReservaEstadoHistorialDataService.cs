using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class ReservaEstadoHistorialDataService : IReservaEstadoHistorialDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public ReservaEstadoHistorialDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<ReservaEstadoHistorialDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.ReservaEstadoHistorial.ListarAsync(cancellationToken)).Select(ReservaEstadoHistorialDataMapper.ToDataModel).ToList();
    public async Task<ReservaEstadoHistorialDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.ReservaEstadoHistorial.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? ReservaEstadoHistorialDataMapper.ToDataModel(e) : null;
}
