namespace AtraccionesTuristicas.Backend.LA.Api.Configuration;

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
            return;

        foreach (var line in File.ReadAllLines(envPath))
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            if (trimmedLine.StartsWith("#"))
                continue;

            var separatorIndex = trimmedLine.IndexOf('=');

            if (separatorIndex <= 0)
                continue;

            var key = trimmedLine[..separatorIndex].Trim();
            var value = trimmedLine[(separatorIndex + 1)..].Trim();

            value = value.Trim('"');

            if (string.IsNullOrWhiteSpace(key))
                continue;

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}