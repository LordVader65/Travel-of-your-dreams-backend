using System.Net.Http.Json;

namespace TravelDreams.ApiGateway.Audit;

public sealed class AuditHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditHttpClient> _logger;

    public AuditHttpClient(HttpClient httpClient, ILogger<AuditHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task TryPublishAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/internal/v1/auditoria/logs", auditEvent, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Audit event was not accepted. StatusCode={StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Audit event could not be sent.");
        }
    }
}
