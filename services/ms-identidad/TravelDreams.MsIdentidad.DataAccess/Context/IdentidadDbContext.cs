using Microsoft.EntityFrameworkCore;
using TravelDreams.MsIdentidad.DataAccess.Entities;

namespace TravelDreams.MsIdentidad.DataAccess.Context;

public sealed class IdentidadDbContext : DbContext
{
    public IdentidadDbContext(DbContextOptions<IdentidadDbContext> options)
        : base(options)
    {
    }

    public DbSet<UsuarioEntity> Usuarios => Set<UsuarioEntity>();
    public DbSet<RolEntity> Roles => Set<RolEntity>();
    public DbSet<UsuarioRolEntity> UsuarioRoles => Set<UsuarioRolEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentidadDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
