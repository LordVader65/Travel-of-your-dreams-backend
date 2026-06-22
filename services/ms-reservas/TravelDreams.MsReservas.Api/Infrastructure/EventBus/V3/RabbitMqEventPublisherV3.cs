using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TravelDreams.MsReservas.Business.Events.V3;

namespace TravelDreams.MsReservas.Api.Infrastructure.EventBus.V3;

public sealed class RabbitMqEventPublisherV3 : IReservaEventPublisherV3, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RabbitMqOptionsV3 _options;
    private readonly ILogger<RabbitMqEventPublisherV3> _logger;
    private readonly object _sync = new();
    private IConnection? _connection;
    private IModel? _channel;
    private bool _initialized;

    public RabbitMqEventPublisherV3(IOptions<RabbitMqOptionsV3> options, ILogger<RabbitMqEventPublisherV3> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync(ReservaV3Event integrationEvent, string routingKey, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("RabbitMQ V3 deshabilitado. Evento {EventType} no publicado.", integrationEvent.EventType);
            return Task.CompletedTask;
        }

        try
        {
            EnsureInitialized();
            if (_channel is null) return Task.CompletedTask;

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent, JsonOptions));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = integrationEvent.EventId.ToString();
            properties.CorrelationId = integrationEvent.CorrelationId.ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Type = integrationEvent.EventType;

            _channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Publicado evento RabbitMQ V3 {EventType} con routing key {RoutingKey}.", integrationEvent.EventType, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo publicar evento RabbitMQ V3 {EventType}. El flujo principal continua.", integrationEvent.EventType);
        }

        return Task.CompletedTask;
    }

    private void EnsureInitialized()
    {
        if (_initialized) return;

        lock (_sync)
        {
            if (_initialized) return;

            var factory = RabbitMqConnectionFactoryV3.Create(_options);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

            DeclareQueue(_options.AuditoriaQueueName, "reserva.v3.*");
            DeclareQueue(_options.ReservasDebugQueueName, "reserva.v3.*");
            DeclareQueue(_options.DeadLetterQueueName, "failed.v3.*");

            _initialized = true;
            _logger.LogInformation("RabbitMQ V3 inicializado en exchange {Exchange}.", _options.ExchangeName);
        }
    }

    private void DeclareQueue(string queueName, string routingKey)
    {
        if (_channel is null) return;
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queueName, _options.ExchangeName, routingKey);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
