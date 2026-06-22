using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Events.V3;
using TravelDreams.MsFacturacion.Business.Interfaces;
using TravelDreams.MsFacturacion.DataAccess.Context;
using TravelDreams.MsFacturacion.DataAccess.Entities.V3;

namespace TravelDreams.MsFacturacion.Api.Infrastructure.EventBus.V3;

public sealed class RabbitMqPagoSolicitudConsumerV3 : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly HashSet<string> TerminalStates = ["FACTURA_EMITIDA", "PAGO_RECHAZADO"];

    private readonly RabbitMqOptionsV3 _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqPagoSolicitudConsumerV3> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqPagoSolicitudConsumerV3(
        IOptions<RabbitMqOptionsV3> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqPagoSolicitudConsumerV3> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Consumidor de solicitudes de pago V3 deshabilitado.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Initialize();
                var consumer = new AsyncEventingBasicConsumer(_channel!);
                consumer.Received += OnMessageReceivedAsync;
                _channel!.BasicConsume(_options.SolicitudesQueueName, autoAck: false, consumer);
                _logger.LogInformation("Consumidor V3 escuchando {Queue}.", _options.SolicitudesQueueName);
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo iniciar el consumidor de pagos V3. Se reintentara.");
                DisposeConnection();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel is null) return;
        var rawBody = Encoding.UTF8.GetString(ea.Body.ToArray());

        try
        {
            var command = JsonSerializer.Deserialize<MarketplacePagoSolicitadoV3>(rawBody, JsonOptions)
                ?? throw new InvalidOperationException("Solicitud de pago V3 vacia.");
            Validate(command);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FacturacionDbContext>();
            var existing = await db.MarketplacePagoSolicitudesV3
                .FirstOrDefaultAsync(x => x.fsol_correlation_id == command.CorrelationId);

            if (existing is not null &&
                (TerminalStates.Contains(existing.fsol_estado) ||
                 existing.fsol_estado == "PROCESANDO" && existing.fsol_updated_at_utc > DateTime.UtcNow.AddMinutes(-2)))
            {
                _logger.LogInformation("Solicitud de pago V3 duplicada omitida: {CorrelationId}.", command.CorrelationId);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
                return;
            }

            var tracking = existing ?? new MarketplacePagoSolicitudV3Entity
            {
                fsol_correlation_id = command.CorrelationId,
                cli_guid = command.Payload.ClienteGuid,
                rev_guid = command.Payload.ReservaGuid,
                fsol_payload_json = rawBody,
                fsol_created_at_utc = DateTime.UtcNow
            };
            tracking.fsol_estado = "PROCESANDO";
            tracking.fsol_updated_at_utc = DateTime.UtcNow;
            if (existing is null) db.MarketplacePagoSolicitudesV3.Add(tracking);
            await db.SaveChangesAsync();

            try
            {
                var service = scope.ServiceProvider.GetRequiredService<IPagoService>();
                var result = await service.ConfirmarPagoYGenerarFacturaAsync(command.Payload.ReservaGuid, new ConfirmarPagoRequest
                {
                    CorrelationId = command.CorrelationId,
                    ClienteGuid = command.Payload.ClienteGuid,
                    DatosFacturacionGuid = command.Payload.DatosFacturacionGuid,
                    Metodo = string.IsNullOrWhiteSpace(command.Payload.Metodo) ? "TARJETA" : command.Payload.Metodo,
                    Monto = command.Payload.Monto,
                    Moneda = command.Payload.Moneda,
                    Referencia = command.Payload.Referencia,
                    OrigenCanal = "MOBILE",
                    Observacion = command.Payload.Observacion
                });

                tracking.fsol_estado = "FACTURA_EMITIDA";
                tracking.fac_guid = result.Guid;
                tracking.fac_numero = result.Numero;
                tracking.fsol_error = null;
            }
            catch (Exception ex)
            {
                tracking.fsol_estado = "PAGO_RECHAZADO";
                tracking.fsol_error = PublicError(ex);
                await PublishRejectedAsync(scope, command, tracking.fsol_error);
            }

            tracking.fsol_updated_at_utc = DateTime.UtcNow;
            await db.SaveChangesAsync();
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Solicitud de pago V3 con JSON invalido descartada.");
            _channel.BasicReject(ea.DeliveryTag, requeue: false);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Fallo transitorio guardando seguimiento de pago V3.");
            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Solicitud de pago V3 invalida descartada.");
            _channel.BasicReject(ea.DeliveryTag, requeue: false);
        }
    }

    private static async Task PublishRejectedAsync(IServiceScope scope, MarketplacePagoSolicitadoV3 command, string error)
    {
        var publisher = scope.ServiceProvider.GetRequiredService<IFacturacionEventPublisherV3>();
        await publisher.PublishAsync(new FacturacionV3Event
        {
            EventType = "facturacion.v3.pago_rechazado",
            CorrelationId = command.CorrelationId,
            Payload = new FacturacionEventPayloadV3
            {
                ReservaGuid = command.Payload.ReservaGuid,
                ClienteGuid = command.Payload.ClienteGuid,
                Metodo = command.Payload.Metodo,
                Total = command.Payload.Monto,
                Moneda = command.Payload.Moneda ?? "USD",
                PagoEstado = "RECHAZADO",
                OrigenCanal = "MOBILE",
                Observacion = error
            }
        }, "facturacion.v3.pago_rechazado");
    }

    private static void Validate(MarketplacePagoSolicitadoV3 command)
    {
        if (command.EventId == Guid.Empty || command.CorrelationId == Guid.Empty)
            throw new InvalidOperationException("EventId y CorrelationId son obligatorios.");
        if (command.Payload.ClienteGuid == Guid.Empty || command.Payload.ReservaGuid == Guid.Empty)
            throw new InvalidOperationException("Cliente y reserva son obligatorios.");
        if (command.Payload.Monto <= 0)
            throw new InvalidOperationException("El monto debe ser positivo.");
    }

    private static string PublicError(Exception ex)
    {
        var message = ex.Message;
        return string.IsNullOrWhiteSpace(message) ? "No se pudo procesar el pago." : message[..Math.Min(message.Length, 500)];
    }

    private void Initialize()
    {
        if (_channel is not null) return;
        var factory = RabbitMqConnectionFactoryV3.Create(_options);
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
        var arguments = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = _options.ExchangeName,
            ["x-dead-letter-routing-key"] = "failed.v3.pago"
        };
        _channel.QueueDeclare(_options.SolicitudesQueueName, durable: true, exclusive: false, autoDelete: false, arguments);
        _channel.QueueBind(_options.SolicitudesQueueName, _options.ExchangeName, "marketplace.v3.pago.solicitado");
        _channel.BasicQos(0, _options.PrefetchCount, global: false);
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

public sealed class MarketplacePagoSolicitadoV3
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid CorrelationId { get; set; }
    public string Source { get; set; } = string.Empty;
    public MarketplacePagoPayloadV3 Payload { get; set; } = new();
}

public sealed class MarketplacePagoPayloadV3
{
    public Guid ClienteGuid { get; set; }
    public Guid ReservaGuid { get; set; }
    public Guid? DatosFacturacionGuid { get; set; }
    public string Metodo { get; set; } = "TARJETA";
    public decimal Monto { get; set; }
    public string? Moneda { get; set; }
    public string? Referencia { get; set; }
    public string? Observacion { get; set; }
}
