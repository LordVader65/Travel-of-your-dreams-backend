using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;

public sealed class CategoriaDataService : ICategoriaDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoriaDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<CategoriaDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Categorias.ListarAsync(cancellationToken)).Select(CategoriaDataMapper.ToDataModel).ToList();
    public async Task<CategoriaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.Categorias.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? CategoriaDataMapper.ToDataModel(e) : null;
    public async Task<CategoriaDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _unitOfWork.Categorias.ObtenerPorGuidAsync(guid, cancellationToken)) is { } e ? CategoriaDataMapper.ToDataModel(e) : null;
    public async Task<CategoriaDataModel> CrearAsync(CategoriaDataModel model, CancellationToken cancellationToken = default) { var e = CategoriaDataMapper.ToEntity(model); await _unitOfWork.Categorias.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return CategoriaDataMapper.ToDataModel(e); }
    public async Task<CategoriaDataModel> ActualizarAsync(CategoriaDataModel model, CancellationToken cancellationToken = default) { var e = CategoriaDataMapper.ToEntity(model); _unitOfWork.Categorias.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return CategoriaDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Categorias.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Categorias.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
