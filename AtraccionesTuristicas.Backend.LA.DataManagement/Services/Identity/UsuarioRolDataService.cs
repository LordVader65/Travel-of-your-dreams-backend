using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Identity;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Identity;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Identity;

public sealed class UsuarioRolDataService : IUsuarioRolDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public UsuarioRolDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<UsuarioRolDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.UsuarioRoles.ListarAsync(cancellationToken)).Select(UsuarioRolDataMapper.ToDataModel).ToList();
    public async Task<IReadOnlyList<UsuarioRolDataModel>> ListarPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default) => (await _unitOfWork.UsuarioRoles.ListarPorUsuarioIdAsync(usuarioId, cancellationToken)).Select(UsuarioRolDataMapper.ToDataModel).ToList();
    public async Task<UsuarioRolDataModel> CrearAsync(UsuarioRolDataModel model, CancellationToken cancellationToken = default) { var e = UsuarioRolDataMapper.ToEntity(model); await _unitOfWork.UsuarioRoles.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return UsuarioRolDataMapper.ToDataModel(e); }
    public async Task<IReadOnlyList<UsuarioRolDataModel>> ReemplazarRolesAsync(int usuarioId, IReadOnlyList<int> rolIds, CancellationToken cancellationToken = default)
    {
        var actuales = await _unitOfWork.UsuarioRoles.ListarPorUsuarioIdAsync(usuarioId, cancellationToken);
        foreach (var actual in actuales)
        {
            _unitOfWork.UsuarioRoles.Remover(actual);
        }

        foreach (var rolId in rolIds.Distinct())
        {
            await _unitOfWork.UsuarioRoles.AgregarAsync(UsuarioRolDataMapper.ToEntity(new UsuarioRolDataModel { UsuarioId = usuarioId, RolId = rolId, Estado = "A" }), cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await ListarPorUsuarioIdAsync(usuarioId, cancellationToken);
    }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.UsuarioRoles.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.UsuarioRoles.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
