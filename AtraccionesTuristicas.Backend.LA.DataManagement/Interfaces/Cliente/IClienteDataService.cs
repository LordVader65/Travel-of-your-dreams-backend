using AtraccionesTuristicas.Backend.LA.DataManagement.Common;
using AtraccionesTuristicas.Backend.LA.DataManagement.Models.Cliente;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces.Cliente;

public interface IClienteDataService
{
    Task<DataPagedResult<ClienteDataModel>> ListarAsync(ClienteFiltroDataModel filtro, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<ClienteDataModel> CrearAsync(ClienteDataModel model, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ActualizarAsync(ClienteDataModel model, CancellationToken cancellationToken = default);
    Task<bool> CambiarEstadoAsync(Guid guid, string estado, string usuario, string ip, CancellationToken cancellationToken = default);
    Task<bool> EliminarLogicamenteAsync(Guid guid, string usuario, string ip, CancellationToken cancellationToken = default);
}
