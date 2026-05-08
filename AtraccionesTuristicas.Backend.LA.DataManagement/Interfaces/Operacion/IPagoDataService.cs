using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Operacion;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Operacion;

public interface IPagoDataService
{
    Task<PagoDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<DataPagedResult<PagoDataModel>> ListarPorReservaAsync(Guid reservaGuid, int page, int limit, CancellationToken cancellationToken = default);
    Task<DataPagedResult<PagoDataModel>> ListarAsync(PagoFiltroDataModel filtro, CancellationToken cancellationToken = default);
    Task<Guid> ConfirmarPagoAsync(PagoCrearDataModel model, CancellationToken cancellationToken = default);
}
