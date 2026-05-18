using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelDreams.MsReservas.DataAccess.Context;

public sealed class ReservasDbContextFactory : IDesignTimeDbContextFactory<ReservasDbContext>
{
    public ReservasDbContext CreateDbContext(string[] args)
    {
        LoadEnvFile();
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__ReservasDb")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=ms_reservas_db;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<ReservasDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ReservasDbContext(options);
    }

    private static void LoadEnvFile()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var possiblePaths = new[]
        {
            Path.Combine(currentDirectory, "services", "ms-reservas", ".env"),
            Path.Combine(currentDirectory, ".env"),
            Path.Combine(currentDirectory, "..", ".env"),
            Path.Combine(currentDirectory, "..", "..", ".env")
        };

        var envPath = possiblePaths.FirstOrDefault(File.Exists);
        if (envPath is null) return;

        foreach (var rawLine in File.ReadAllLines(envPath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0) continue;
            Environment.SetEnvironmentVariable(line[..separatorIndex].Trim(), line[(separatorIndex + 1)..].Trim().Trim('"'));
        }
    }
}
