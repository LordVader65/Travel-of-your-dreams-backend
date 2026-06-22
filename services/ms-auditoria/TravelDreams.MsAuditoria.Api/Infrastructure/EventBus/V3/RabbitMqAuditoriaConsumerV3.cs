using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TravelDreams.MsAuditoria.Business.DTOs;
using TravelDreams.MsAuditoria.Business.Interfaces;

namespace TravelDreams.MsAuditoria.Api.Infrastructure.EventBus.V3;

public sealed class RabbitMqAuditoriaConsumerV3 : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RabbitMqOptionsV3 _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqAuditoriaConsumerV3> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqAuditoriaConsumerV3(
        IOptions<RabbitMqOptionsV3> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqAuditoriaConsumerV3> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("RabbitMQ V3 deshabilitado para auditoria.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EnsureInitialized();
                if (_channel is null) return;

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += OnMessageReceivedAsync;

                _channel.BasicConsume(
                    queue: _options.QueueName,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("Consumidor RabbitMQ V3 de auditoria escuchando cola {Queue}.", _options.QueueName);
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo iniciar el consumidor RabbitMQ V3 de auditoria. Se reintentara.");
                DisposeConnection();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel is null) return;

        var body = Encoding.UTF8.GetString(ea.Body.ToArray());
        try
        {
            var integrationEvent = JsonSerializer.Deserialize<IntegrationEventV3>(body, JsonOptions);
            if (integrationEvent?.EventId is null || integrationEvent.EventId == Guid.Empty)
            {
                _logger.LogWarning("Mensaje RabbitMQ V3 descartado por no tener EventId valido.");
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
                return;
            }

            await ProcessEventAsync(integrationEvent, body, ea.RoutingKey);
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Mensaje RabbitMQ V3 descartado por JSON invalido.");
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Mensaje RabbitMQ V3 descartado por validacion de auditoria.");
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos procesando evento RabbitMQ V3. Se reencola.");
            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado procesando evento RabbitMQ V3. Se reencola.");
            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
        }

        return;
    }

    private async Task ProcessEventAsync(IntegrationEventV3 integrationEvent, string rawBody, string routingKey)
    {
        using var scope = _scopeFactory.CreateScope();
        var auditoria = scope.ServiceProvider.GetRequiredService<IAuditoriaLogService>();

        var table = ResolveTable(integrationEvent.EventType, routingKey);
        var registroGuid = ResolveRegistroGuid(integrationEvent.Payload, table);
        var origenCanal = GetString(integrationEvent.Payload, "origenCanal");
        var id = await auditoria.RegistrarAsync(new RegistrarAuditoriaRequest
        {
            EventoId = integrationEvent.EventId,
            Servicio = string.IsNullOrWhiteSpace(integrationEvent.Source) ? "ms-reservas" : integrationEvent.Source,
            Tabla = table,
            Operacion = "BUSINESS_EVENT",
            RegistroGuid = registroGuid,
            DatosNuevos = rawBody,
            FechaUtc = integrationEvent.OccurredAtUtc == default ? DateTime.UtcNow : integrationEvent.OccurredAtUtc,
            Usuario = "rabbitmq-v3",
            Ip = "rabbitmq",
            OrigenCanal = origenCanal,
            CorrelationId = integrationEvent.CorrelationId == Guid.Empty ? null : integrationEvent.CorrelationId.ToString()
        });

        if (id == 0)
        {
            _logger.LogInformation("Evento RabbitMQ V3 duplicado omitido: {EventId}.", integrationEvent.EventId);
            return;
        }

        _logger.LogInformation(
            "Evento RabbitMQ V3 auditado: {EventType} ({RoutingKey}) para registro {RegistroGuid}.",
            integrationEvent.EventType,
            routingKey,
            registroGuid);
    }

    private static string ResolveTable(string eventType, string routingKey)
    {
        var key = string.IsNullOrWhiteSpace(eventType) ? routingKey : eventType;
        if (key.Contains("factura", StringComparison.OrdinalIgnoreCase)) return "facturas";
        if (key.Contains("pago", StringComparison.OrdinalIgnoreCase)) return "pagos";
        if (key.Contains("atraccion", StringComparison.OrdinalIgnoreCase)) return "atracciones";
        if (key.Contains("disponibilidad", StringComparison.OrdinalIgnoreCase)) return "disponibilidad";
        if (key.Contains("ticket", StringComparison.OrdinalIgnoreCase)) return "tickets";
        if (key.Contains("horario", StringComparison.OrdinalIgnoreCase)) return "horarios";
        if (key.Contains("catalogo", StringComparison.OrdinalIgnoreCase)) return "catalogos";
        if (key.Contains("resenia", StringComparison.OrdinalIgnoreCase)) return "resenias";
        if (key.Contains("marketplace", StringComparison.OrdinalIgnoreCase)) return "marketplace_solicitudes";
        return "reservas";
    }

    private static Guid? ResolveRegistroGuid(JsonElement payload, string table)
    {
        var candidates = table switch
        {
            "facturas" => new[] { "facturaGuid", "FacturaGuid", "guid" },
            "pagos" => new[] { "pagoGuid", "PagoGuid", "guid" },
            _ => new[] { "reservaGuid", "ReservaGuid", "guid" }
        };

        foreach (var candidate in candidates)
        {
            if (TryGetGuid(payload, candidate, out var guid)) return guid;
        }

        return null;
    }

    private static string? GetString(JsonElement payload, string propertyName)
    {
        if (payload.ValueKind != JsonValueKind.Object) return null;
        return payload.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static bool TryGetGuid(JsonElement payload, string propertyName, out Guid guid)
    {
        guid = Guid.Empty;
        if (payload.ValueKind != JsonValueKind.Object) return false;
        if (!payload.TryGetProperty(propertyName, out var value)) return false;
        if (value.ValueKind != JsonValueKind.String) return false;
        return Guid.TryParse(value.GetString(), out guid) && guid != Guid.Empty;
    }

    private void EnsureInitialized()
    {
        if (_channel is not null) return;

        var factory = RabbitMqConnectionFactoryV3.Create(_options);

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
        _channel.QueueDeclare(_options.QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "reserva.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "facturacion.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "marketplace.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "atraccion.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "disponibilidad.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "ticket.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "horario.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "catalogo.v3.*");
        _channel.QueueBind(_options.QueueName, _options.ExchangeName, "resenia.v3.*");
        _channel.BasicQos(prefetchSize: 0, prefetchCount: _options.PrefetchCount, global: false);
    }

    public override void Dispose()
    {
        DisposeConnection();
        base.Dispose();
    }

    private void DisposeConnection()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _channel = null;
        _connection = null;
    }
}
