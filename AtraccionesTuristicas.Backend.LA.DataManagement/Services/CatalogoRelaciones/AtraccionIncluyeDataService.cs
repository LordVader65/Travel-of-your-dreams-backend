using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.CatalogoRelaciones;

public sealed class AtraccionIncluyeDataService : IAtraccionIncluyeDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public AtraccionIncluyeDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<AtraccionIncluyeDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.AtraccionIncluyes.ListarAsync(cancellationToken)).Select(AtraccionIncluyeDataMapper.ToDataModel).ToList();
    public async Task<AtraccionIncluyeDataModel> CrearAsync(AtraccionIncluyeDataModel model, CancellationToken cancellationToken = default) { var e = AtraccionIncluyeDataMapper.ToEntity(model); await _unitOfWork.AtraccionIncluyes.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return AtraccionIncluyeDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.AtraccionIncluyes.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.AtraccionIncluyes.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
    public async Task RemoverAsync(int atraccionId, int incluyeId, CancellationToken cancellationToken = default) { var e = await _unitOfWork.AtraccionIncluyes.ObtenerPorRelacionAsync(atraccionId, incluyeId, cancellationToken); if (e is null) return; _unitOfWork.AtraccionIncluyes.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
