namespace TravelDreams.ApiGateway.Configuration;

public static class EnvLoader
{
    public static void Load()
    {
        var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
        if (!File.Exists(envPath))
        {
            envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        }

        if (!File.Exists(envPath)) return;

        foreach (var rawLine in File.ReadAllLines(envPath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0) continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(key)) continue;
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
