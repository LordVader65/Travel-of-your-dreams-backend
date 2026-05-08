using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class ReservaDetalleDataService : IReservaDetalleDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public ReservaDetalleDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<ReservaDetalleDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.ReservaDetalles.ListarAsync(cancellationToken)).Select(ReservaDetalleDataMapper.ToDataModel).ToList();
    public async Task<ReservaDetalleDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.ReservaDetalles.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? ReservaDetalleDataMapper.ToDataModel(e) : null;
}
