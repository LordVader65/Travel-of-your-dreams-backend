using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class PagoDataService : IPagoDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public PagoDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagoDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Pagos.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : PagoDataMapper.ToDataModel(entity);
    }

    public async Task<DataPagedResult<PagoDataModel>> ListarPorReservaAsync(Guid reservaGuid, int page, int limit, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PagoQueries.ListarPorReservaAsync(reservaGuid, page, limit, cancellationToken);
        return new DataPagedResult<PagoDataModel>
        {
            Items = result.Items.Select(PagoDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public async Task<DataPagedResult<PagoDataModel>> ListarAsync(PagoFiltroDataModel filtro, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PagoQueries.ListarAsync(
            filtro.ReservaGuid,
            filtro.ClienteGuid,
            filtro.Metodo,
            filtro.Estado,
            filtro.FechaDesdeUtc,
            filtro.FechaHastaUtc,
            filtro.Page,
            filtro.Limit,
            cancellationToken);

        return new DataPagedResult<PagoDataModel>
        {
            Items = result.Items.Select(PagoDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public Task<Guid> ConfirmarPagoAsync(PagoCrearDataModel model, CancellationToken cancellationToken = default) =>
        _unitOfWork.Pagos.ConfirmarPagoAsync(
            model.ReservaGuid,
            model.Metodo,
            model.Monto,
            model.Referencia,
            model.Usuario,
            model.Ip,
            model.OrigenCanal,
            cancellationToken);
}
