namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;

public interface IDestinoService { Task<IReadOnlyList<DestinoResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<DestinoResponse> CrearAsync(CrearDestinoRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<DestinoResponse> ActualizarAsync(ActualizarDestinoRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

