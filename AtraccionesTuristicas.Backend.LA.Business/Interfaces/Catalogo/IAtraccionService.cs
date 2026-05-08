namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;

public interface IAtraccionService
    {
        Task<BusinessPagedResult<AtraccionPublicaResponse>> ListarPublicasAsync(AtraccionFiltroRequest filtro, CancellationToken cancellationToken = default);
        Task<AtraccionPublicaResponse> ObtenerDetallePublicoAsync(Guid guid, CancellationToken cancellationToken = default);
        Task<AtraccionDetalleResponse> ObtenerDetalleCompletoAsync(Guid guid, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default);
        Task<AtraccionResponse> CrearAsync(CrearAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
        Task<AtraccionResponse> ActualizarAsync(ActualizarAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default);
        Task<BusinessOperationResult> EliminarAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default);
    }

