namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;

public interface IIncluyeService { Task<IReadOnlyList<IncluyeResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<IncluyeResponse> CrearAsync(CrearIncluyeRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<IncluyeResponse> ActualizarAsync(ActualizarIncluyeRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

