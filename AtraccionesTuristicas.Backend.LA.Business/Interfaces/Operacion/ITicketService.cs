namespace AtraccionesTuristicas.Backend.LA.Business.Interfaces.Operacion;

public interface ITicketService { Task<IReadOnlyList<TicketResponse>> ListarAsync(CancellationToken cancellationToken = default); Task<IReadOnlyList<TicketResponse>> ListarActivosPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default); Task<TicketResponse> CrearAsync(CrearTicketRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<TicketResponse> ActualizarAsync(ActualizarTicketRequest request, CurrentUserData user, CancellationToken cancellationToken = default); Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default); }

