using AtraccionesTuristicas.Backend.LA.DataAccess.Common;
using AtraccionesTuristicas.Backend.LA.DataAccess.Context;
using AtraccionesTuristicas.Backend.LA.DataAccess.Entities.Cliente;
using AtraccionesTuristicas.Backend.LA.DataAccess.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Queries;

public sealed class ClienteQueryRepository : IClienteQueryRepository
{
    private readonly AtraccionesDbContext _context;

    public ClienteQueryRepository(AtraccionesDbContext context) => _context = context;

    public async Task<PagedResult<ClienteEntity>> ListarAsync(string? numeroIdentificacion = null, string? correo = null, string? estado = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.Clientes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(numeroIdentificacion))
            query = query.Where(x => x.cli_numero_identificacion.Contains(numeroIdentificacion));
        if (!string.IsNullOrWhiteSpace(correo))
            query = query.Where(x => x.cli_correo.Contains(correo));
        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.cli_estado == estado);

        query = query.OrderBy(x => x.cli_id);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);
        return new PagedResult<ClienteEntity> { Items = items, Page = page, Limit = limit, Total = total };
    }

    public Task<ClienteEntity?> ObtenerPorGuidAsync(Guid guid, CancellationToken cancellationToken = default) =>
        _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.cli_guid == guid, cancellationToken);
}
