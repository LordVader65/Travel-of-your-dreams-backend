using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class ReseniaDataService : IReseniaDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public ReseniaDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<ReseniaDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Resenias.ListarAsync(cancellationToken)).Select(ReseniaDataMapper.ToDataModel).ToList();
    public async Task<ReseniaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) => (await _unitOfWork.Resenias.ObtenerPorIdAsync(id, cancellationToken)) is { } e ? ReseniaDataMapper.ToDataModel(e) : null;
    public async Task<ReseniaDataModel> CrearAsync(ReseniaDataModel model, CancellationToken cancellationToken = default) { var e = ReseniaDataMapper.ToEntity(model); await _unitOfWork.Resenias.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return ReseniaDataMapper.ToDataModel(e); }
    public async Task<ReseniaDataModel> ActualizarAsync(ReseniaDataModel model, CancellationToken cancellationToken = default) { var e = ReseniaDataMapper.ToEntity(model); _unitOfWork.Resenias.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return ReseniaDataMapper.ToDataModel(e); }
    public async Task<ReseniaDataModel?> CambiarEstadoAsync(int id, string estado, string usuario, string ip, CancellationToken cancellationToken = default)
    {
        var e = await _unitOfWork.Resenias.ObtenerPorIdAsync(id, cancellationToken);
        if (e is null) return null;
        e.rsn_estado = estado;
        e.rsn_fecha_mod = DateTime.UtcNow;
        e.rsn_usuario_mod = usuario;
        e.rsn_ip_mod = ip;
        if (estado == "I")
        {
            e.rsn_fecha_eliminacion = DateTime.UtcNow;
            e.rsn_usuario_eliminacion = usuario;
            e.rsn_ip_eliminacion = ip;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ReseniaDataMapper.ToDataModel(e);
    }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Resenias.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Resenias.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
