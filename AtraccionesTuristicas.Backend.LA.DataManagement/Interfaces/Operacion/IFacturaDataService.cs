using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IFacturaDataService
{
    Task<FacturaDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<FacturaDataModel?> ObtenerPorNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<DataPagedResult<FacturaDataModel>> ListarPorClienteAsync(Guid clienteGuid, int page, int limit, CancellationToken cancellationToken = default);
    Task<DataPagedResult<FacturaDataModel>> ListarAsync(FacturaFiltroDataModel filtro, CancellationToken cancellationToken = default);
    Task<Guid> GenerarFacturaAsync(Guid reservaGuid, Guid? datosFacturacionGuid, string usuario, string ip, string? observacion = null, string? origenCanal = null, CancellationToken cancellationToken = default);
}
