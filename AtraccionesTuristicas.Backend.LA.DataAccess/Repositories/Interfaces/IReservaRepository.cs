using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IReservaRepository : IRepositoryBase<ReservaEntity>
{
    Task<ReservaEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ReservaEntity?> ObtenerPorIdConReseniaAsync(int id, CancellationToken cancellationToken = default);
    Task<ReservaEntity?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<ReservaEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ReservaEntity?> ObtenerConDetalleAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<Guid> CrearReservaAsync(
        Guid clienteGuid,
        Guid horarioGuid,
        string ticketsJson,
        string usuario,
        string ip,
        string? origenCanal = null,
        int expiracionMinutos = 15,
        decimal porcentajeIva = 0,
        CancellationToken cancellationToken = default);
    Task CancelarReservaAsync(Guid reservaGuid, string usuario, string ip, string motivo, CancellationToken cancellationToken = default);
    Task CambiarEstadoAsync(Guid reservaGuid, string nuevoEstado, string usuario, string ip, string? observacion = null, CancellationToken cancellationToken = default);
    Task<int> ExpirarReservasPendientesAsync(string usuario = "system", string ip = "127.0.0.1", CancellationToken cancellationToken = default);
}
