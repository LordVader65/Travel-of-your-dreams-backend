using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.CatalogoRelaciones;

public sealed class CategoriaAtraccionDataService : ICategoriaAtraccionDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoriaAtraccionDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<CategoriaAtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.CategoriaAtracciones.ListarAsync(cancellationToken)).Select(CategoriaAtraccionDataMapper.ToDataModel).ToList();
    public async Task<CategoriaAtraccionDataModel> CrearAsync(CategoriaAtraccionDataModel model, CancellationToken cancellationToken = default) { var e = CategoriaAtraccionDataMapper.ToEntity(model); await _unitOfWork.CategoriaAtracciones.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return CategoriaAtraccionDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.CategoriaAtracciones.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.CategoriaAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
    public async Task RemoverAsync(int atraccionId, int categoriaId, CancellationToken cancellationToken = default) { var e = await _unitOfWork.CategoriaAtracciones.ObtenerPorRelacionAsync(atraccionId, categoriaId, cancellationToken); if (e is null) return; _unitOfWork.CategoriaAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
