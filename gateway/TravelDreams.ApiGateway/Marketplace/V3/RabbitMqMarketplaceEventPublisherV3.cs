using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace TravelDreams.ApiGateway.Marketplace.V3;

public sealed class RabbitMqMarketplaceEventPublisherV3 : IMarketplaceEventPublisherV3, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly RabbitMqOptionsV3 _options;
    private readonly ILogger<RabbitMqMarketplaceEventPublisherV3> _logger;
    private readonly object _sync = new();
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqMarketplaceEventPublisherV3(
        IOptions<RabbitMqOptionsV3> options,
        ILogger<RabbitMqMarketplaceEventPublisherV3> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync(MarketplaceIntegrationEventV3 integrationEvent, string routingKey, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (!_options.Enabled)
            throw new InvalidOperationException("RabbitMQ V3 esta deshabilitado en el API Gateway.");

        EnsureInitialized();
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent, JsonOptions));
        var properties = _channel!.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.MessageId = integrationEvent.EventId.ToString();
        properties.CorrelationId = integrationEvent.CorrelationId.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Type = integrationEvent.EventType;

        _channel.BasicPublish(_options.ExchangeName, routingKey, mandatory: false, properties, body);
        _logger.LogInformation("Comando marketplace V3 {EventType} publicado con correlation {CorrelationId}.", integrationEvent.EventType, integrationEvent.CorrelationId);
        return Task.CompletedTask;
    }

    private void EnsureInitialized()
    {
        if (_channel is not null) return;
        lock (_sync)
        {
            if (_channel is not null) return;
            var factory = RabbitMqConnectionFactoryV3.Create(_options);
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
            Declare(_options.ReservasQueueName, "marketplace.v3.reserva.solicitada", "failed.v3.reserva");
            Declare(_options.FacturacionQueueName, "marketplace.v3.pago.solicitado", "failed.v3.pago");
            Declare(_options.MarketplaceDebugQueueName, "marketplace.v3.*");
            Declare(_options.AuditoriaQueueName, "marketplace.v3.*");
            Declare(_options.DeadLetterQueueName, "failed.v3.*");
        }
    }

    private void Declare(string queue, string binding, string? deadLetterRoutingKey = null)
    {
        IDictionary<string, object>? arguments = null;
        if (!string.IsNullOrWhiteSpace(deadLetterRoutingKey))
        {
            arguments = new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = _options.ExchangeName,
                ["x-dead-letter-routing-key"] = deadLetterRoutingKey
            };
        }

        _channel!.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments);
        _channel.QueueBind(queue, _options.ExchangeName, binding);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
