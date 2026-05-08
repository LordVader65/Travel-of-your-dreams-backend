namespace AtraccionesTuristicas.Backend.LA.Business.Services.Operacion;

public sealed class TicketService : ITicketService
    {
        private readonly ITicketDataService _data; public TicketService(ITicketDataService data) => _data = data;
        public async Task<IReadOnlyList<TicketResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Ticket).ToList();
        public async Task<IReadOnlyList<TicketResponse>> ListarActivosPorAtraccionAsync(int atraccionId, CancellationToken cancellationToken = default) => (await _data.ListarActivosPorAtraccionAsync(atraccionId, cancellationToken)).Select(Map.Ticket).ToList();
        public async Task<TicketResponse> CrearAsync(CrearTicketRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request); return Map.Ticket(await _data.CrearAsync(Map.Ticket(request), cancellationToken)); }
        public async Task<TicketResponse> ActualizarAsync(ActualizarTicketRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request); var m = Map.Ticket((CrearTicketRequest)request); m.Guid = request.Guid; m.Estado = request.Estado; return Map.Ticket(await _data.ActualizarAsync(m, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        private static void Validate(CrearTicketRequest request) { var errors = new List<string>(); Support.Guard.Positive(request.AtraccionId, "AtraccionId", errors); Support.Guard.Required(request.Titulo, "Titulo", errors); Support.Guard.Positive(request.Precio, "Precio", errors); Support.Guard.Positive(request.CapacidadMaxima, "CapacidadMaxima", errors); Support.Guard.ThrowIfAny(errors); }
    }

