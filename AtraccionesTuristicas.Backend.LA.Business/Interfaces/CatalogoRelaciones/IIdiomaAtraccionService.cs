namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.CatalogoRelaciones;

public interface IIdiomaAtraccionService { Task<IReadOnlyList<IdiomaAtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<IdiomaAtraccionResponse> AsociarAsync(AsociarIdiomaAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> RemoverAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

