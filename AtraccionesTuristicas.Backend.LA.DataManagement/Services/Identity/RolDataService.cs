using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Identity;

public sealed class RolDataService : IRolDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public RolDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<RolDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Roles.ListarAsync(cancellationToken)).Select(RolDataMapper.ToDataModel).ToList();
    public async Task<RolDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _unitOfWork.Roles.ObtenerPorGuidAsync(guid, cancellationToken)) is { } e ? RolDataMapper.ToDataModel(e) : null;
    public async Task<RolDataModel?> ObtenerPorDescripcionAsync(string descripcion, CancellationToken cancellationToken = default) => (await _unitOfWork.Roles.ObtenerPorDescripcionAsync(descripcion, cancellationToken)) is { } e ? RolDataMapper.ToDataModel(e) : null;
    public async Task<RolDataModel> CrearAsync(RolDataModel model, CancellationToken cancellationToken = default) { var e = RolDataMapper.ToEntity(model); await _unitOfWork.Roles.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return RolDataMapper.ToDataModel(e); }
    public async Task<RolDataModel> ActualizarAsync(RolDataModel model, CancellationToken cancellationToken = default) { var e = RolDataMapper.ToEntity(model); _unitOfWork.Roles.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return RolDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Roles.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Roles.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
