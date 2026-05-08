using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AtraccionesTuristicas.Backend.LA.DataAccess.Context;

public sealed class AtraccionesDbContextFactory : IDesignTimeDbContextFactory<AtraccionesDbContext>
{
    public AtraccionesDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AtraccionesDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Defina ConnectionStrings__AtraccionesDb para ejecutar migraciones EF Core.");
        }

        var options = new DbContextOptionsBuilder<AtraccionesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AtraccionesDbContext(options);
    }
}
