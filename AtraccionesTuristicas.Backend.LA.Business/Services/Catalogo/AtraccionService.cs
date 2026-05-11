namespace AtraccionesTuristicas.Backend.LA.Business.Services.Catalogo;

public sealed class AtraccionService : IAtraccionService
    {
        private readonly IAtraccionDataService _data;
        private readonly ICategoriaAtraccionDataService _categoriaAtracciones;
        private readonly IIdiomaAtraccionDataService _idiomaAtracciones;
        private readonly IImagenAtraccionDataService _imagenAtracciones;
        private readonly IAtraccionIncluyeDataService _atraccionIncluyes;
        private readonly ICategoriaDataService _categorias;
        private readonly IIdiomaDataService _idiomas;
        private readonly IImagenDataService _imagenes;
        private readonly IIncluyeDataService _incluyes;
        private readonly ITicketDataService _tickets;
        private readonly IReseniaDataService _resenias;
        public AtraccionService(IAtraccionDataService data, ICategoriaAtraccionDataService categoriaAtracciones, IIdiomaAtraccionDataService idiomaAtracciones, IImagenAtraccionDataService imagenAtracciones, IAtraccionIncluyeDataService atraccionIncluyes, ICategoriaDataService categorias, IIdiomaDataService idiomas, IImagenDataService imagenes, IIncluyeDataService incluyes, ITicketDataService tickets, IReseniaDataService resenias)
        {
            _data = data; _categoriaAtracciones = categoriaAtracciones; _idiomaAtracciones = idiomaAtracciones; _imagenAtracciones = imagenAtracciones; _atraccionIncluyes = atraccionIncluyes; _categorias = categorias; _idiomas = idiomas; _imagenes = imagenes; _incluyes = incluyes; _tickets = tickets; _resenias = resenias;
        }
        public async Task<BusinessPagedResult<AtraccionPublicaResponse>> ListarPublicasAsync(AtraccionFiltroRequest filtro, CancellationToken cancellationToken = default)
        {
            var result = await _data.ListarPublicasAsync(new AtraccionFiltroDataModel { Pais = filtro.Pais, FechaDesde = filtro.FechaDesde, FechaHasta = filtro.FechaHasta, Tipo = filtro.Tipo, Subtipo = filtro.Subtipo, Etiqueta = filtro.Etiqueta, Idioma = filtro.Idioma, PrecioMinimo = filtro.PrecioMinimo, PrecioMaximo = filtro.PrecioMaximo, RatingMinimo = filtro.RatingMinimo, Horario = filtro.Horario, OrdenarPor = filtro.OrdenarPor, SoloDisponibles = filtro.SoloDisponibles, Page = filtro.Page, Limit = filtro.Limit }, cancellationToken);
            return Support.Paging.Map(result, Map.AtraccionPublica);
        }
        public async Task<AtraccionPublicaResponse> ObtenerDetallePublicoAsync(Guid guid, CancellationToken cancellationToken = default) => Map.AtraccionPublica(await _data.ObtenerDetallePublicoAsync(guid, cancellationToken) ?? throw new NotFoundException("Atraccion no encontrada."));
        public async Task<AtraccionDetalleResponse> ObtenerDetalleCompletoAsync(Guid guid, CancellationToken cancellationToken = default)
        {
            var publicData = await _data.ObtenerDetallePublicoAsync(guid, cancellationToken) ?? throw new NotFoundException("Atraccion no encontrada.");
            var baseResponse = Map.AtraccionPublica(publicData);
            var atraccionId = baseResponse.Id;

            var categoriaLinks = (await _categoriaAtracciones.ListarAsync(cancellationToken)).Where(x => x.AtraccionId == atraccionId && x.Estado == BusinessConstants.EstadoActivo).Select(x => x.CategoriaId).ToHashSet();
            var idiomaLinks = (await _idiomaAtracciones.ListarAsync(cancellationToken)).Where(x => x.AtraccionId == atraccionId && x.Estado == BusinessConstants.EstadoActivo).Select(x => x.IdiomaId).ToHashSet();
            var imagenLinks = (await _imagenAtracciones.ListarAsync(cancellationToken)).Where(x => x.AtraccionId == atraccionId && x.Estado == BusinessConstants.EstadoActivo).Select(x => x.ImagenId).ToHashSet();
            var incluyeLinks = (await _atraccionIncluyes.ListarAsync(cancellationToken)).Where(x => x.AtraccionId == atraccionId && x.Estado == BusinessConstants.EstadoActivo).Select(x => x.IncluyeId).ToHashSet();

            return new AtraccionDetalleResponse
            {
                Id = baseResponse.Id, Guid = baseResponse.Guid, Nombre = baseResponse.Nombre, Descripcion = baseResponse.Descripcion, Pais = baseResponse.Pais, Direccion = baseResponse.Direccion, DuracionMinutos = baseResponse.DuracionMinutos, PrecioReferencia = baseResponse.PrecioReferencia, Disponible = baseResponse.Disponible, FreeCancellation = baseResponse.FreeCancellation, SkipTheLine = baseResponse.SkipTheLine, TotalResenias = baseResponse.TotalResenias,
                Categorias = (await _categorias.ListarAsync(cancellationToken)).Where(x => categoriaLinks.Contains(x.Id) && x.Estado == BusinessConstants.EstadoActivo).Select(Map.Categoria).ToList(),
                Idiomas = (await _idiomas.ListarAsync(cancellationToken)).Where(x => idiomaLinks.Contains(x.Id) && x.Estado == BusinessConstants.EstadoActivo).Select(Map.Idioma).ToList(),
                Imagenes = (await _imagenes.ListarAsync(cancellationToken)).Where(x => imagenLinks.Contains(x.Id) && x.Estado == BusinessConstants.EstadoActivo).Select(Map.Imagen).ToList(),
                Incluye = (await _incluyes.ListarAsync(cancellationToken)).Where(x => incluyeLinks.Contains(x.Id) && x.Estado == BusinessConstants.EstadoActivo).Select(Map.Incluye).ToList(),
                Tickets = (await _tickets.ListarActivosPorAtraccionAsync(atraccionId, cancellationToken)).Select(Map.Ticket).ToList(),
                Resenias = (await _resenias.ListarAsync(cancellationToken)).Where(x => x.AtraccionId == atraccionId && x.Estado == BusinessConstants.EstadoActivo).Select(Map.Resenia).ToList()
            };
        }
        public async Task<IReadOnlyList<AtraccionResponse>> ListarAsync(CancellationToken cancellationToken = default) => (await _data.ListarAsync(cancellationToken)).Select(Map.Atraccion).ToList();
        public async Task<AtraccionResponse> CrearAsync(CrearAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request); return Map.Atraccion(await _data.CrearAsync(Map.Atraccion(request), cancellationToken)); }
        public async Task<AtraccionResponse> ActualizarAsync(ActualizarAtraccionRequest request, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); Validate(request); return Map.Atraccion(await _data.ActualizarAsync(Map.Atraccion(request), cancellationToken) ?? throw new NotFoundException("Atraccion no encontrada.")); }
        public async Task<BusinessOperationResult> EliminarAsync(Guid guid, CurrentUserData user, CancellationToken cancellationToken = default) { Support.Guard.EnsureAdmin(user); var item = await _data.ObtenerPorGuidAsync(guid, cancellationToken) ?? throw new NotFoundException("Atraccion no encontrada."); await _data.RemoverAsync(item.Id, cancellationToken); return BusinessOperationResult.Ok("Atraccion eliminada o inactivada."); }
        private static void Validate(CrearAtraccionRequest request) { var errors = new List<string>(); Support.Guard.Required(request.Nombre, "Nombre", errors); Support.Guard.Positive(request.DestinoId, "DestinoId", errors); Support.Guard.NonNegative(request.DuracionMinutos, "DuracionMinutos", errors); Support.Guard.NonNegative(request.PrecioReferencia, "PrecioReferencia", errors); Support.Guard.MaxLength(request.NumeroEstablecimiento, 30, "NumeroEstablecimiento", errors); Support.Guard.MaxLength(request.Nombre, 200, "Nombre", errors); Support.Guard.MaxLength(request.Descripcion, 2000, "Descripcion", errors); Support.Guard.MaxLength(request.Direccion, 300, "Direccion", errors); Support.Guard.MaxLength(request.PuntoEncuentro, 300, "PuntoEncuentro", errors); Support.Guard.ThrowIfAny(errors); }
    }
