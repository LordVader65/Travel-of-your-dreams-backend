using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;

public interface IClienteQueryRepository
{
    Task<PagedResult<ClienteEntity>> ListarAsync(string? numeroIdentificacion = null, string? correo = null, string? estado = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}
