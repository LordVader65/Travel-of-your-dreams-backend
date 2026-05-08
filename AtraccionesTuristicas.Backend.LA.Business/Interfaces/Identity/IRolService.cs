namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Identity;

public interface IRolService { Task<IReadOnlyList<RolResponse>> ListarAsync(CancellationToken cancellationToken = default); }

