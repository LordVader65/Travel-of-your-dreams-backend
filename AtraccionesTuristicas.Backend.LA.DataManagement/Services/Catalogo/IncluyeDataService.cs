using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;

public sealed class IncluyeDataService : IIncluyeDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public IncluyeDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<IncluyeDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Incluyes.ListarAsync(cancellationToken)).Select(IncluyeDataMapper.ToDataModel).ToList();
    public async Task<IncluyeDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.Incluyes.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? IncluyeDataMapper.ToDataModel(e) : null;
    public async Task<IncluyeDataModel> CrearAsync(IncluyeDataModel model, CancellationToken cancellationToken = default) { var e = IncluyeDataMapper.ToEntity(model); await _unitOfWork.Incluyes.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return IncluyeDataMapper.ToDataModel(e); }
    public async Task<IncluyeDataModel> ActualizarAsync(IncluyeDataModel model, CancellationToken cancellationToken = default) { var e = IncluyeDataMapper.ToEntity(model); _unitOfWork.Incluyes.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return IncluyeDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Incluyes.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Incluyes.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
