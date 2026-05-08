namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;

public interface ICategoriaService { Task<IReadOnlyList<CategoriaResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<CategoriaResponse> CrearAsync(CrearCategoriaRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<CategoriaResponse> ActualizarAsync(ActualizarCategoriaRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

