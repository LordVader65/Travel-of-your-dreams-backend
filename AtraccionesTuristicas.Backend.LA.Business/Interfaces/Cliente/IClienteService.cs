namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;

public interface IClienteService
    {
        Task<BusinessPagedResult<ClienteResponse>> ListarAsync(ClienteFiltroRequest filtro, CancellationToken cancellationToken = default);
        Task<ClienteResponse> CrearAsync(CrearClienteRequest request, CancellationToken cancellationToken = default);
        Task<ClienteResponse?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default);
        Task<ClienteResponse?> ObtenerPorIdentificacionAsync(string numeroIdentificacion, CancellationToken cancellationToken = default);
        Task<ClienteResponse> ActualizarAsync(ActualizarClienteRequest request, CancellationToken cancellationToken = default);
        Task<BusinessOperationResult> CambiarEstadoAsync(CambiarEstadoClienteRequest request, CancellationToken cancellationToken = default);
    }

