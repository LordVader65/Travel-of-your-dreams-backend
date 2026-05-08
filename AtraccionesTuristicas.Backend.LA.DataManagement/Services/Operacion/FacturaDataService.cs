using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class FacturaDataService : IFacturaDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public FacturaDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<FacturaDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Facturas.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : FacturaDataMapper.ToDataModel(entity);
    }

    public async Task<FacturaDataModel?> ObtenerPorNumeroAsync(string numero, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FacturaQueries.ObtenerPorNumeroAsync(numero, cancellationToken);
        return entity is null ? null : FacturaDataMapper.ToDataModel(entity);
    }

    public async Task<DataPagedResult<FacturaDataModel>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.FacturaQueries.ListarPorClienteAsync(clienteGuid, page, limit, cancellationToken);
        return new DataPagedResult<FacturaDataModel>
        {
            Items = result.Items.Select(FacturaDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public async Task<DataPagedResult<FacturaDataModel>> ListarAsync(FacturaFiltroDataModel filtro, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.FacturaQueries.ListarAsync(
            filtro.ClienteGuid,
            filtro.ReservaGuid,
            filtro.Numero,
            filtro.Estado,
            filtro.FechaDesdeUtc,
            filtro.FechaHastaUtc,
            filtro.Page,
            filtro.Limit,
            cancellationToken);

        return new DataPagedResult<FacturaDataModel>
        {
            Items = result.Items.Select(FacturaDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public Task<Guid> GenerarFacturaAsync(Guid reservaGuid, Guid? datosFacturacionGuid, string usuario, string ip, string? observacion = null, string? origenCanal = null, CancellationToken cancellationToken = default) =>
        _unitOfWork.Facturas.GenerarFacturaAsync(reservaGuid, datosFacturacionGuid, usuario, ip, observacion, origenCanal, cancellationToken);
}
