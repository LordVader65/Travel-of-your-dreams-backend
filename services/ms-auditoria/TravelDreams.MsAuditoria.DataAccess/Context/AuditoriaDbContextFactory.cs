using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelDreams.MsAuditoria.DataAccess.Context;

public sealed class AuditoriaDbContextFactory : IDesignTimeDbContextFactory<AuditoriaDbContext>
{
    public AuditoriaDbContext CreateDbContext(string[] args)
    {
        LoadEnvFile();
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__AuditoriaDb")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=ms_auditoria_db;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AuditoriaDbContext>().UseNpgsql(connectionString).Options;
        return new AuditoriaDbContext(options);
    }

    private static void LoadEnvFile()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var possiblePaths = new[]
        {
            Path.Combine(currentDirectory, "services", "ms-auditoria", ".env"),
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
