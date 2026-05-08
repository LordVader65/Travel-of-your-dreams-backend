namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Cliente;

public interface IDatosFacturacionService
    {
        Task<IReadOnlyList<DatosFacturacionResponse>> ListarActivosPorClienteAsync(Guid clienteGuid, CurrentUserData user, CancellationToken cancellationToken = default);
        Task<DatosFacturacionResponse> CrearAsync(CrearDatosFacturacionRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
        Task<DatosFacturacionResponse> ActualizarAsync(ActualizarDatosFacturacionRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
        Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default);
    }

