using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Services;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AtraccionesDbContext _context;

    public UnitOfWork(AtraccionesDbContext context)
    {
        _context = context;
        Roles = new RolRepository(context);
        Usuarios = new UsuarioRepository(context);
        UsuarioRoles = new UsuarioRolRepository(context);
        Clientes = new ClienteRepository(context);
        DatosFacturacion = new DatosFacturacionRepository(context);
        Destinos = new DestinoRepository(context);
        Categorias = new CategoriaRepository(context);
        Idiomas = new IdiomaRepository(context);
        Incluyes = new IncluyeRepository(context);
        Imagenes = new ImagenRepository(context);
        Atracciones = new AtraccionRepository(context);
        CategoriaAtracciones = new CategoriaAtraccionRepository(context);
        IdiomaAtracciones = new IdiomaAtraccionRepository(context);
        ImagenAtracciones = new ImagenAtraccionRepository(context);
        AtraccionIncluyes = new AtraccionIncluyeRepository(context);
        Tickets = new TicketRepository(context);
        Horarios = new HorarioRepository(context);
        Reservas = new ReservaRepository(context);
        ReservaDetalles = new ReservaDetalleRepository(context);
        ReservaEstadoHistorial = new ReservaEstadoHistorialRepository(context);
        Pagos = new PagoRepository(context);
        Facturas = new FacturaRepository(context);
        Resenias = new ReseniaRepository(context);
        AuditoriaLogs = new AuditoriaLogRepository(context);

        ClienteQueries = new ClienteQueryRepository(context);
        AtraccionQueries = new AtraccionQueryRepository(context);
        HorarioQueries = new HorarioQueryRepository(context);
        ReservaQueries = new ReservaQueryRepository(context);
        PagoQueries = new PagoQueryRepository(context);
        FacturaQueries = new FacturaQueryRepository(context);
        AuditoriaLogQueries = new AuditoriaLogQueryRepository(context);
    }

    public IRolRepository Roles { get; }
    public IUsuarioRepository Usuarios { get; }
    public IUsuarioRolRepository UsuarioRoles { get; }
    public IClienteRepository Clientes { get; }
    public IDatosFacturacionRepository DatosFacturacion { get; }
    public IDestinoRepository Destinos { get; }
    public ICategoriaRepository Categorias { get; }
    public IIdiomaRepository Idiomas { get; }
    public IIncluyeRepository Incluyes { get; }
    public IImagenRepository Imagenes { get; }
    public IAtraccionRepository Atracciones { get; }
    public ICategoriaAtraccionRepository CategoriaAtracciones { get; }
    public IIdiomaAtraccionRepository IdiomaAtracciones { get; }
    public IImagenAtraccionRepository ImagenAtracciones { get; }
    public IAtraccionIncluyeRepository AtraccionIncluyes { get; }
    public ITicketRepository Tickets { get; }
    public IHorarioRepository Horarios { get; }
    public IReservaRepository Reservas { get; }
    public IReservaDetalleRepository ReservaDetalles { get; }
    public IReservaEstadoHistorialRepository ReservaEstadoHistorial { get; }
    public IPagoRepository Pagos { get; }
    public IFacturaRepository Facturas { get; }
    public IReseniaRepository Resenias { get; }
    public IAuditoriaLogRepository AuditoriaLogs { get; }

    public IClienteQueryRepository ClienteQueries { get; }
    public IAtraccionQueryRepository AtraccionQueries { get; }
    public IHorarioQueryRepository HorarioQueries { get; }
    public IReservaQueryRepository ReservaQueries { get; }
    public IPagoQueryRepository PagoQueries { get; }
    public IFacturaQueryRepository FacturaQueries { get; }
    public IAuditoriaLogQueryRepository AuditoriaLogQueries { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
