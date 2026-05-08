using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;

public sealed class DestinoDataService : IDestinoDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public DestinoDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<DestinoDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Destinos.ListarAsync(cancellationToken)).Select(DestinoDataMapper.ToDataModel).ToList();
    public async Task<DestinoDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.Destinos.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? DestinoDataMapper.ToDataModel(e) : null;
    public async Task<DestinoDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _unitOfWork.Destinos.ObtenerPorGuidAsync(guid, cancellationToken)) is { } e ? DestinoDataMapper.ToDataModel(e) : null;
    public async Task<DestinoDataModel> CrearAsync(DestinoDataModel model, CancellationToken cancellationToken = default) { var e = DestinoDataMapper.ToEntity(model); await _unitOfWork.Destinos.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return DestinoDataMapper.ToDataModel(e); }
    public async Task<DestinoDataModel> ActualizarAsync(DestinoDataModel model, CancellationToken cancellationToken = default) { var e = DestinoDataMapper.ToEntity(model); _unitOfWork.Destinos.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return DestinoDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Destinos.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Destinos.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
