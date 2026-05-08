using System.Text.Json;
using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Mappers.Operacion;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services.Operacion;

public sealed class ReservaDataService : IReservaDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservaDataService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<ReservaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Reservas.ObtenerPorIdConReseniaAsync(id, cancellationToken);
        return entity is null ? null : ReservaDataMapper.ToDataModel(entity);
    }

    public async Task<ReservaDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ReservaQueries.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null ? null : ReservaDataMapper.ToDataModel(entity);
    }

    public async Task<ReservaDataModel?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ReservaQueries.ObtenerPorCodigoAsync(codigo, cancellationToken);
        return entity is null ? null : ReservaDataMapper.ToDataModel(entity);
    }

    public async Task<DataPagedResult<ReservaDataModel>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ReservaQueries.ListarPorClienteAsync(clienteGuid, page, limit, cancellationToken);
        return new DataPagedResult<ReservaDataModel>
        {
            Items = result.Items.Select(ReservaDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public async Task<DataPagedResult<ReservaDataModel>> ListarAsync(ReservaFiltroDataModel filtro, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ReservaQueries.ListarAsync(
            filtro.ClienteGuid,
            filtro.AtraccionGuid,
            filtro.Codigo,
            filtro.Estado,
            filtro.FechaDesde,
            filtro.FechaHasta,
            filtro.Page,
            filtro.Limit,
            cancellationToken);

        return new DataPagedResult<ReservaDataModel>
        {
            Items = result.Items.Select(ReservaDataMapper.ToDataModel).ToList(),
            Page = result.Page,
            Limit = result.Limit,
            Total = result.Total
        };
    }

    public Task<Guid> CrearReservaAsync(ReservaCrearDataModel model, CancellationToken cancellationToken = default)
    {
        var ticketsJson = JsonSerializer.Serialize(model.Tickets.Select(x => new
        {
            tck_guid = x.TicketGuid,
            cantidad = x.Cantidad
        }));

        return _unitOfWork.Reservas.CrearReservaAsync(
            model.ClienteGuid,
            model.HorarioGuid,
            ticketsJson,
            model.Usuario,
            model.Ip,
            model.OrigenCanal,
            model.ExpiracionMinutos,
            model.PorcentajeIva,
            cancellationToken);
    }

    public Task CancelarReservaAsync(Guid reservaGuid, string usuario, string ip, string motivo, CancellationToken cancellationToken = default) =>
        _unitOfWork.Reservas.CancelarReservaAsync(reservaGuid, usuario, ip, motivo, cancellationToken);

    public Task CambiarEstadoAsync(Guid reservaGuid, string nuevoEstado, string usuario, string ip, string? observacion = null, CancellationToken cancellationToken = default) =>
        _unitOfWork.Reservas.CambiarEstadoAsync(reservaGuid, nuevoEstado, usuario, ip, observacion, cancellationToken);

    public Task<int> ExpirarReservasPendientesAsync(string usuario = "system", string ip = "127.0.0.1", CancellationToken cancellationToken = default) =>
        _unitOfWork.Reservas.ExpirarReservasPendientesAsync(usuario, ip, cancellationToken);
}
