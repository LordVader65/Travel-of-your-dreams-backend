using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

public interface IDatosFacturacionRepository : IRepositoryBase<DatosFacturacionEntity>
{
    Task<DatosFacturacionEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DatosFacturacionEntity>> ListarActivosPorClienteAsync(Guid clienteGuid, CancellationToken cancellationToken = default);
    Task<DatosFacturacionEntity?> ObtenerParaActualizarAsync(Guid guid, CancellationToken cancellationToken = default);
}
