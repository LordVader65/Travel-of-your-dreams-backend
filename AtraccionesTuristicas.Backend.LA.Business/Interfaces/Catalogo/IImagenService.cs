namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Catalogo;

public interface IImagenService { Task<IReadOnlyList<ImagenResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<ImagenResponse> CrearAsync(CrearImagenRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<ImagenResponse> ActualizarAsync(ActualizarImagenRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

