namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;

public interface IIdiomaService { Task<IReadOnlyList<IdiomaResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<IdiomaResponse> CrearAsync(CrearIdiomaRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<IdiomaResponse> ActualizarAsync(ActualizarIdiomaRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

