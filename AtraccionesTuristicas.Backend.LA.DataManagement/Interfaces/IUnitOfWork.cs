using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using AtraccionesTuristicas.Backend.LA.DataAccess.Repositories.Interfaces;

namespace AtraccionesTuristicas.Backend.LA.DataManagement.Interfaces;

public interface IUnitOfWork
{
    IRolRepository Roles { get; }
    IUsuarioRepository Usuarios { get; }
    IUsuarioRolRepository UsuarioRoles { get; }
    IClienteRepository Clientes { get; }
    IDatosFacturacionRepository DatosFacturacion { get; }
    IDestinoRepository Destinos { get; }
    ICategoriaRepository Categorias { get; }
    IIdiomaRepository Idiomas { get; }
    IIncluyeRepository Incluyes { get; }
    IImagenRepository Imagenes { get; }
    IAtraccionRepository Atracciones { get; }
    ICategoriaAtraccionRepository CategoriaAtracciones { get; }
    IIdiomaAtraccionRepository IdiomaAtracciones { get; }
    IImagenAtraccionRepository ImagenAtracciones { get; }
    IAtraccionIncluyeRepository AtraccionIncluyes { get; }
    ITicketRepository Tickets { get; }
    IHorarioRepository Horarios { get; }
    IReservaRepository Reservas { get; }
    IReservaDetalleRepository ReservaDetalles { get; }
    IReservaEstadoHistorialRepository ReservaEstadoHistorial { get; }
    IPagoRepository Pagos { get; }
    IFacturaRepository Facturas { get; }
    IReseniaRepository Resenias { get; }
    IAuditoriaLogRepository AuditoriaLogs { get; }

    IClienteQueryRepository ClienteQueries { get; }
    IAtraccionQueryRepository AtraccionQueries { get; }
    IHorarioQueryRepository HorarioQueries { get; }
    IReservaQueryRepository ReservaQueries { get; }
    IPagoQueryRepository PagoQueries { get; }
    IFacturaQueryRepository FacturaQueries { get; }
    IAuditoriaLogQueryRepository AuditoriaLogQueries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
