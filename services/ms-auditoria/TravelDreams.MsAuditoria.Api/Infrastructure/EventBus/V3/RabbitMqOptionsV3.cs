namespace TravelDreams.MsAuditoria.Api.Infrastructure.EventBus.V3;

public sealed class RabbitMqOptionsV3
{
    public bool Enabled { get; set; }
    public string? Uri { get; set; }
    public bool UseTls { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "traveldreams.marketplace.v3.events";
    public string QueueName { get; set; } = "q.v3.auditoria";
    public ushort PrefetchCount { get; set; } = 10;
}
