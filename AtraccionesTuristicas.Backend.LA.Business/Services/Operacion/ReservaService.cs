namespace AtraccionesTuristicas.Backend.LA.Business.Services.Operacion;

public sealed class ReservaService : IReservaService
    {
        private readonly IReservaDataService _data;
        private readonly IClienteDataService _clientes;
        private readonly IHorarioDataService _horarios;
        private readonly ITicketDataService _tickets;
        public ReservaService(IReservaDataService data, IClienteDataService clientes, IHorarioDataService horarios, ITicketDataService tickets)
        {
            _data = data; _clientes = clientes; _horarios = horarios; _tickets = tickets;
        }
        public async Task<ReservaResponse> ObtenerPorGuidAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default) => Map.Reserva(await _data.ObtenerPorGuidAsync(guid, cancellationToken) ?? throw new NotFoundException("Reserva no encontrada."));
        public async Task<ReservaResponse?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancellationToken = default) => (await _data.ObtenerPorCodigoAsync(codigo, cancellationToken)) is { } r ? Map.Reserva(r) : null;
        public async Task<BusinessPagedResult<ReservaResponse>> ListarAsync(ReservaFiltroRequest filtro, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            if (!user.EsAdmin) filtro.ClienteGuid = user.ClienteGuid ?? filtro.ClienteGuid;
            var result = await _data.ListarAsync(new ReservaFiltroDataModel { ClienteGuid = filtro.ClienteGuid, AtraccionGuid = filtro.AtraccionGuid, Codigo = filtro.Codigo, Estado = filtro.Estado, FechaDesde = filtro.FechaDesde, FechaHasta = filtro.FechaHasta, Page = filtro.Page, Limit = filtro.Limit }, cancellationToken);
            return Support.Paging.Map(result, Map.Reserva);
        }
        public async Task<Guid> CrearAsync(CrearReservaRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            var horarioGuid = request.HorarioGuid;
            if (request.Fecha.HasValue)
            {
                var horarioMaterializado = await _horarios.MaterializarParaFechaAsync(request.HorarioGuid, request.Fecha.Value, request.Usuario, request.Ip, cancellationToken)
                    ?? throw new NotFoundException("Horario no encontrado.");
                horarioGuid = horarioMaterializado.Guid;
            }

            var requestMaterializado = new CrearReservaRequest
            {
                ClienteGuid = request.ClienteGuid,
                HorarioGuid = horarioGuid,
                Fecha = request.Fecha,
                Tickets = request.Tickets,
                Usuario = request.Usuario,
                Ip = request.Ip,
                OrigenCanal = request.OrigenCanal,
                ExpiracionMinutos = request.ExpiracionMinutos,
                PorcentajeIva = request.PorcentajeIva
            };

            await PrevisualizarAsync(requestMaterializado, user, cancellationToken);
            return await _data.CrearReservaAsync(new ReservaCrearDataModel { ClienteGuid = request.ClienteGuid, HorarioGuid = horarioGuid, Tickets = request.Tickets.Select(x => new ReservaCrearDetalleDataModel { TicketGuid = x.TicketGuid, Cantidad = x.Cantidad }).ToList(), Usuario = request.Usuario, Ip = request.Ip, OrigenCanal = request.OrigenCanal, ExpiracionMinutos = request.ExpiracionMinutos, PorcentajeIva = request.PorcentajeIva }, cancellationToken);
        }
        public async Task<PrevisualizarReservaResponse> PrevisualizarAsync(CrearReservaRequest request, CurrentUserData user, CancellationToken cancellationToken = default)
        {
            if (!user.EsAdmin) Support.Guard.EnsureOwnerOrAdmin(user, request.ClienteGuid);
            var errors = new List<string>(); if (request.ClienteGuid == Guid.Empty) errors.Add("ClienteGuid es obligatorio."); if (request.HorarioGuid == Guid.Empty) errors.Add("HorarioGuid es obligatorio."); if (request.Tickets.Count == 0) errors.Add("Debe seleccionar al menos un ticket."); foreach (var t in request.Tickets) { if (t.TicketGuid == Guid.Empty) errors.Add("TicketGuid es obligatorio."); Support.Guard.Positive(t.Cantidad, "Cantidad", errors); } Support.Guard.Positive(request.ExpiracionMinutos, "ExpiracionMinutos", errors); Support.Guard.NonNegative(request.PorcentajeIva, "PorcentajeIva", errors); Support.Guard.ThrowIfAny(errors);

            var cliente = await _clientes.ObtenerPorGuidAsync(request.ClienteGuid, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
            if (cliente.Estado != BusinessConstants.EstadoActivo) throw new BusinessRuleException("Cliente inactivo.");
            var horario = await _horarios.ObtenerPorGuidAsync(request.HorarioGuid, cancellationToken) ?? throw new NotFoundException("Horario no encontrado.");
            if (request.Fecha.HasValue) horario.Fecha = request.Fecha.Value;
            if (horario.Estado != BusinessConstants.EstadoActivo) throw new BusinessRuleException("Horario inactivo.");
            if (horario.Fecha < DateOnly.FromDateTime(DateTime.UtcNow)) throw new BusinessRuleException("No se puede reservar un horario vencido.");
            if (horario.Fecha == DateOnly.FromDateTime(DateTime.Now) && horario.HoraInicio <= TimeOnly.FromDateTime(DateTime.Now)) throw new BusinessRuleException("No se puede reservar un horario que ya paso.");

            var totalCantidad = request.Tickets.Sum(x => x.Cantidad);
            if (horario.CuposDisponibles < totalCantidad) throw new ConflictBusinessException("No existen cupos suficientes.");

            var detalles = new List<PrevisualizarReservaDetalleResponse>();
            foreach (var item in request.Tickets)
            {
                var ticket = await _tickets.ObtenerPorGuidAsync(item.TicketGuid, cancellationToken) ?? throw new NotFoundException("Ticket no encontrado.");
                if (ticket.Estado != BusinessConstants.EstadoActivo) throw new BusinessRuleException("Ticket inactivo.");
                if (ticket.AtraccionId != horario.AtraccionId) throw new BusinessRuleException("El ticket no pertenece a la atraccion del horario.");
                if (item.Cantidad > ticket.CapacidadMaxima) throw new BusinessRuleException($"La cantidad para {ticket.Titulo} supera el maximo permitido ({ticket.CapacidadMaxima}).");
                detalles.Add(new PrevisualizarReservaDetalleResponse { TicketGuid = ticket.Guid, Titulo = ticket.Titulo, TipoParticipante = ticket.TipoParticipante, Cantidad = item.Cantidad, PrecioUnitario = ticket.Precio, Subtotal = ticket.Precio * item.Cantidad });
            }

            var subtotal = detalles.Sum(x => x.Subtotal);
            var iva = Math.Round(subtotal * request.PorcentajeIva / 100, 2, MidpointRounding.AwayFromZero);
            return new PrevisualizarReservaResponse { ClienteGuid = request.ClienteGuid, HorarioGuid = request.HorarioGuid, Fecha = horario.Fecha, HoraInicio = horario.HoraInicio, HoraFin = horario.HoraFin, CuposDisponibles = horario.CuposDisponibles, Subtotal = subtotal, ValorIva = iva, Total = subtotal + iva, Moneda = BusinessConstants.MonedaDefault, Detalles = detalles };
        }
        public async Task<BusinessOperationResult> CancelarAsync(CancelarReservaRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { var errors = new List<string>(); Support.Guard.Required(request.Motivo, "Motivo", errors); Support.Guard.ThrowIfAny(errors); await _data.CancelarReservaAsync(request.ReservaGuid, request.Usuario, request.Ip, request.Motivo, cancellationToken); return BusinessOperationResult.Ok("Reserva cancelada."); }
        public async Task<BusinessOperationResult> CambiarEstadoAdminAsync(CambiarEstadoReservaAdminRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var allowed = new[] { BusinessConstants.ReservaPendiente, BusinessConstants.ReservaPagada, BusinessConstants.ReservaConfirmada, BusinessConstants.ReservaCancelada, BusinessConstants.ReservaExpirada, BusinessConstants.ReservaUsada, BusinessConstants.ReservaNoShow }; if (!allowed.Contains(request.Estado)) throw new BusinessRuleException("Estado de reserva no permitido."); await _data.CambiarEstadoAsync(request.ReservaGuid, request.Estado, request.Usuario, request.Ip, request.Observacion, cancellationToken); return BusinessOperationResult.Ok("Estado actualizado."); }
        public Task<int> ExpirarPendientesAsync(CurrentUserData user, CancellationToken cancellationToken = default) => _data.ExpirarReservasPendientesAsync(user.Login, user.Ip, cancellationToken);
    }

