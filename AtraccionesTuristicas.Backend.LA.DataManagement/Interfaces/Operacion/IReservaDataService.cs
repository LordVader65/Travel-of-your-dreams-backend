using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IReservaDataService
{
    Task<ReservaDataModel?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReservaDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ReservaDataModel?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<DataPagedResult<ReservaDataModel>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default);
    Task<DataPagedResult<ReservaDataModel>> ListarAsync(ReservaFiltroDataModel filtro, CancellationToken cancellationToken = default);
    Task<Guid> CrearReservaAsync(ReservaCrearDataModel model, CancellationToken cancellationToken = default);
    Task CancelarReservaAsync(Guid reservaGuid, string usuario, string ip, string motivo, CancellationToken cancellationToken = default);
    Task CambiarEstadoAsync(Guid reservaGuid, string nuevoEstado, string usuario, string ip, string? observacion = null, CancellationToken cancellationToken = default);
    Task<int> ExpirarReservasPendientesAsync(string usuario = "system", string ip = "127.0.0.1", CancellationToken cancellationToken = default);
}
