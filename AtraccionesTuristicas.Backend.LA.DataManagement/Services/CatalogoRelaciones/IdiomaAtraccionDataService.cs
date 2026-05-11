using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.CatalogoRelaciones;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.CatalogoRelaciones;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.CatalogoRelaciones;

public sealed class IdiomaAtraccionDataService : IIdiomaAtraccionDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public IdiomaAtraccionDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<IdiomaAtraccionDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.IdiomaAtracciones.ListarAsync(cancellationToken)).Select(IdiomaAtraccionDataMapper.ToDataModel).ToList();
    public async Task<IdiomaAtraccionDataModel> CrearAsync(IdiomaAtraccionDataModel model, CancellationToken cancellationToken = default) { var e = IdiomaAtraccionDataMapper.ToEntity(model); await _unitOfWork.IdiomaAtracciones.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return IdiomaAtraccionDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.IdiomaAtracciones.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.IdiomaAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
    public async Task RemoverAsync(int atraccionId, int idiomaId, CancellationToken cancellationToken = default) { var e = await _unitOfWork.IdiomaAtracciones.ObtenerPorRelacionAsync(atraccionId, idiomaId, cancellationToken); if (e is null) return; _unitOfWork.IdiomaAtracciones.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
