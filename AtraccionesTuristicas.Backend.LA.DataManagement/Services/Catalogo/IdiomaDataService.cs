using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Catalogo;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Catalogo;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Catalogo;

public sealed class IdiomaDataService : IIdiomaDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public IdiomaDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<IdiomaDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Idiomas.ListarAsync(cancellationToken)).Select(IdiomaDataMapper.ToDataModel).ToList();
    public async Task<IdiomaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.Idiomas.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? IdiomaDataMapper.ToDataModel(e) : null;
    public async Task<IdiomaDataModel?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default) => (await _unitOfWork.Idiomas.ObtenerPorCodigoAsync(codigo, cancellationToken)) is { } e ? IdiomaDataMapper.ToDataModel(e) : null;
    public async Task<IdiomaDataModel> CrearAsync(IdiomaDataModel model, CancellationToken cancellationToken = default) { var e = IdiomaDataMapper.ToEntity(model); await _unitOfWork.Idiomas.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return IdiomaDataMapper.ToDataModel(e); }
    public async Task<IdiomaDataModel> ActualizarAsync(IdiomaDataModel model, CancellationToken cancellationToken = default) { var e = IdiomaDataMapper.ToEntity(model); _unitOfWork.Idiomas.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return IdiomaDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Idiomas.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Idiomas.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
