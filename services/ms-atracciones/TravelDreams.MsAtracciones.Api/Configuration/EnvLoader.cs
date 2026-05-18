namespace TravelDreams.MsAtracciones.Api.Configuration;

public static class EnvLoader
{
    public static void Load(string fileName = ".env")
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var possiblePaths = new[]
        {
            Path.Combine(currentDirectory, fileName),
            Path.Combine(currentDirectory, "..", fileName),
            Path.Combine(currentDirectory, "..", "..", fileName)
        };

        var envPath = possiblePaths.FirstOrDefault(File.Exists);
        if (envPath is null)
        {
            return;
        }

        foreach (var rawLine in File.ReadAllLines(envPath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');
            if (!string.IsNullOrWhiteSpace(key))
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
