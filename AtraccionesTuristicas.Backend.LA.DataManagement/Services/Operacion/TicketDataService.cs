using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class TicketDataService : ITicketDataService
{
    private readonly IUnitOfWork _unitOfWork;
    public TicketDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<IReadOnlyList<TicketDataModel>> ListarAsync(CancellationToken cancellationToken = default) => (await _unitOfWork.Tickets.ListarAsync(cancellationToken)).Select(TicketDataMapper.ToDataModel).ToList();
    public async Task<TicketDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) => (await _unitOfWork.Tickets.ObtenerPorGuidAsync(guid, cancellationToken)) is { } e ? TicketDataMapper.ToDataModel(e) : null;
    public async Task<IReadOnlyList<TicketDataModel>> ListarActivosPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default) => (await _unitOfWork.Tickets.ListarActivosPorAtraccionAsync(atraccionId, cancellationToken)).Select(TicketDataMapper.ToDataModel).ToList();
    public async Task<TicketDataModel> CrearAsync(TicketDataModel model, CancellationToken cancellationToken = default) { var e = TicketDataMapper.ToEntity(model); await _unitOfWork.Tickets.AgregarAsync(e, cancellationToken); await _unitOfWork.SaveChangesAsync(cancellationToken); return TicketDataMapper.ToDataModel(e); }
    public async Task<TicketDataModel> ActualizarAsync(TicketDataModel model, CancellationToken cancellationToken = default) { var e = TicketDataMapper.ToEntity(model); _unitOfWork.Tickets.Actualizar(e); await _unitOfWork.SaveChangesAsync(cancellationToken); return TicketDataMapper.ToDataModel(e); }
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default) { var e = await _unitOfWork.Tickets.ObtenerPorIdAsync(id, cancellationToken); if (e is null) return; _unitOfWork.Tickets.Remover(e); await _unitOfWork.SaveChangesAsync(cancellationToken); }
}
