namespace AtraccionesTuristicas.Backend.LA.Business.Services.Operacion;

public sealed class ReseniaService : IReseniaService
    {
        private readonly IReseniaDataService _data;
        private readonly IReservaDataService _reservas;
        public ReseniaService(IReseniaDataService data, IReservaDataService reservas) { _data = data; _reservas = reservas; }
        public async Task<IReadOnlyList<ReseniaResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Where(x => x.Estado == BusinessConstants.EstadoActivo).Select(Map.Resenia).ToList();
        public async Task<IReadOnlyList<ReseniaResponse>> ListarAdminAsync(CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); return (await _data.ListarAsync(cancellationToken)).Select(Map.Resenia).ToList(); }
        public async Task<ReseniaResponse> CrearAsync(CrearReseniaRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { await ValidateAsync(request, cancellationToken); return Map.Resenia(await _data.CrearAsync(new ReseniaDataModel { AtraccionId = request.AtraccionId, ReservaId = request.ReservaId, Comentario = request.Comentario, Rating = request.Rating, UsuarioCreacion = request.UsuarioCreacion, IpCreacion = request.IpCreacion, Estado = BusinessConstants.EstadoActivo }, cancellationToken)); }
        public async Task<ReseniaResponse> ActualizarAsync(CrearReseniaRequest request, int id, CurrentUserData user, CancellationToken cancellationToken = default) { Validate(request); return Map.Resenia(await _data.ActualizarAsync(new ReseniaDataModel { Id = id, AtraccionId = request.AtraccionId, ReservaId = request.ReservaId, Comentario = request.Comentario, Rating = request.Rating, UsuarioCreacion = request.UsuarioCreacion, IpCreacion = request.IpCreacion, Estado = BusinessConstants.EstadoActivo }, cancellationToken)); }
        public async Task<BusinessOperationResult> EliminarAsync(int id, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); await _data.RemoverAsync(id, cancellationToken); return BusinessOperationResult.Ok(); }
        public async Task<BusinessOperationResult> CambiarEstadoAsync(int id, string estado, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            Support.Guard.EnsureAdmin(user);
            if (estado is not (BusinessConstants.EstadoActivo or BusinessConstants.EstadoInactivo)) throw new ValidationException(["Estado de resenia no permitido."]);
            _ = await _data.CambiarEstadoAsync(id, estado, user.Login, user.Ip, cancellationToken) ?? throw new NotFoundException("Resenia no encontrada.");
            return BusinessOperationResult.Ok("Estado de resenia actualizado.");
        }
        private static void Validate(CrearReseniaRequest request) { var errors = new List<string>(); Support.Guard.Positive(request.AtraccionId, "AtraccionId", errors); Support.Guard.Positive(request.ReservaId, "ReservaId", errors); if (request.Rating < 1 || request.Rating > 5) errors.Add("Rating debe estar entre 1 y 5."); Support.Guard.ThrowIfAny(errors); }
        private async Task ValidateAsync(CrearReseniaRequest request, CancellationToken cancellationToken)
        {
            Validate(request);
            var reserva = await _reservas.ObtenerPorIdAsync(request.ReservaId, cancellationToken) ?? throw new BusinessRuleException("La reserva no existe.");
            if (reserva.Estado != BusinessConstants.ReservaUsada) throw new BusinessRuleException("Solo se puede reseniar una reserva usada.");
            if ((await _data.ListarAsync(cancellationToken)).Any(x => x.ReservaId == request.ReservaId && x.Estado == BusinessConstants.EstadoActivo)) throw new ConflictBusinessException("La reserva ya tiene una resenia activa.");
        }
    }

