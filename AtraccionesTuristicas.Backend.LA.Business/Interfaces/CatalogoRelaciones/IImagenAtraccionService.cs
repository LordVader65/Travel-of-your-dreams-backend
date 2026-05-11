namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.CatalogoRelaciones;

public interface IImagenAtraccionService { Task<IReadOnlyList<ImagenAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<ImagenAtraccionResponse> AsociarAsync(AsociarImagenAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> RemoverAsync(int atraccionId, int imagenId, CurrentUserData user, CancellationToken cancellationToken = default); }

