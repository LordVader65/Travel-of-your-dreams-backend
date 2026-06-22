using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Events.V3;
using TravelDreams.MsReservas.Business.Interfaces;
using TravelDreams.MsReservas.DataAccess.Context;
using TravelDreams.MsReservas.DataAccess.Entities.V3;

namespace TravelDreams.MsReservas.Api.Infrastructure.EventBus.V3;

public sealed class RabbitMqReservaSolicitudConsumerV3 : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly HashSet<string> TerminalStates = ["RESERVA_CREADA", "RESERVA_RECHAZADA"];

    private readonly RabbitMqOptionsV3 _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqReservaSolicitudConsumerV3> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqReservaSolicitudConsumerV3(
        IOptions<RabbitMqOptionsV3> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqReservaSolicitudConsumerV3> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Consumidor de solicitudes de reserva V3 deshabilitado.");
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
                _logger.LogError(ex, "No se pudo iniciar el consumidor de reservas V3. Se reintentara.");
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
            var command = JsonSerializer.Deserialize<MarketplaceReservaSolicitadaV3>(rawBody, JsonOptions)
                ?? throw new InvalidOperationException("Solicitud de reserva V3 vacia.");
            Validate(command);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ReservasDbContext>();
            var existing = await db.MarketplaceReservaSolicitudesV3
                .FirstOrDefaultAsync(x => x.rsol_correlation_id == command.CorrelationId);

            if (existing is not null &&
                (TerminalStates.Contains(existing.rsol_estado) ||
                 existing.rsol_estado == "PROCESANDO" && existing.rsol_updated_at_utc > DateTime.UtcNow.AddMinutes(-2)))
            {
                _logger.LogInformation("Solicitud de reserva V3 duplicada omitida: {CorrelationId}.", command.CorrelationId);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
                return;
            }

            var tracking = existing ?? new MarketplaceReservaSolicitudV3Entity
            {
                rsol_correlation_id = command.CorrelationId,
                cli_guid = command.Payload.ClienteGuid,
                rsol_payload_json = rawBody,
                rsol_created_at_utc = DateTime.UtcNow
            };
            tracking.rsol_estado = "PROCESANDO";
            tracking.rsol_updated_at_utc = DateTime.UtcNow;
            if (existing is null) db.MarketplaceReservaSolicitudesV3.Add(tracking);
            await db.SaveChangesAsync();

            try
            {
                var service = scope.ServiceProvider.GetRequiredService<IReservasService>();
                var result = await service.CrearAsync(new CrearReservaRequest
                {
                    CorrelationId = command.CorrelationId,
                    ClienteGuid = command.Payload.ClienteGuid,
                    AtraccionGuid = command.Payload.AtraccionGuid,
                    HorarioGuid = command.Payload.HorarioGuid,
                    Lineas = command.Payload.Lineas.Select(x => new CrearReservaLineaRequest
                    {
                        TicketGuid = x.TicketGuid,
                        Cantidad = x.Cantidad
                    }).ToList(),
                    OrigenCanal = "MOBILE",
                    ExpiracionMinutos = command.Payload.ExpiracionMinutos is > 0 and <= 120
                        ? command.Payload.ExpiracionMinutos
                        : 15,
                    PorcentajeIva = command.Payload.PorcentajeIva
                });

                tracking.rsol_estado = "RESERVA_CREADA";
                tracking.rev_guid = result.Guid;
                tracking.rev_codigo = result.Codigo;
                tracking.rsol_error = null;
            }
            catch (Exception ex)
            {
                tracking.rsol_estado = "RESERVA_RECHAZADA";
                tracking.rsol_error = PublicError(ex);
                await PublishRejectedAsync(scope, command, tracking.rsol_error);
            }

            tracking.rsol_updated_at_utc = DateTime.UtcNow;
            await db.SaveChangesAsync();
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Solicitud de reserva V3 con JSON invalido descartada.");
            _channel.BasicReject(ea.DeliveryTag, requeue: false);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Fallo transitorio guardando seguimiento de reserva V3.");
            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Solicitud de reserva V3 invalida descartada.");
            _channel.BasicReject(ea.DeliveryTag, requeue: false);
        }
    }

    private static async Task PublishRejectedAsync(IServiceScope scope, MarketplaceReservaSolicitadaV3 command, string error)
    {
        var publisher = scope.ServiceProvider.GetRequiredService<IReservaEventPublisherV3>();
        await publisher.PublishAsync(new ReservaV3Event
        {
            EventType = "reserva.v3.rechazada",
            CorrelationId = command.CorrelationId,
            Payload = new ReservaEventPayloadV3
            {
                ClienteGuid = command.Payload.ClienteGuid,
                AtraccionGuid = command.Payload.AtraccionGuid,
                HorarioGuid = command.Payload.HorarioGuid,
                Estado = "RECHAZADA",
                OrigenCanal = "MOBILE",
                Motivo = error
            }
        }, "reserva.v3.rechazada");
    }

    private static void Validate(MarketplaceReservaSolicitadaV3 command)
    {
        if (command.EventId == Guid.Empty || command.CorrelationId == Guid.Empty)
            throw new InvalidOperationException("EventId y CorrelationId son obligatorios.");
        if (command.Payload.ClienteGuid == Guid.Empty || command.Payload.AtraccionGuid == Guid.Empty || command.Payload.HorarioGuid == Guid.Empty)
            throw new InvalidOperationException("Cliente, atraccion y horario son obligatorios.");
        if (command.Payload.Lineas.Count == 0)
            throw new InvalidOperationException("Debe incluir al menos una linea de reserva.");
    }

    private static string PublicError(Exception ex)
    {
        var message = ex.Message;
        return string.IsNullOrWhiteSpace(message) ? "No se pudo procesar la reserva." : message[..Math.Min(message.Length, 500)];
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
            ["x-dead-letter-routing-key"] = "failed.v3.reserva"
        };
        _channel.QueueDeclare(_options.SolicitudesQueueName, durable: true, exclusive: false, autoDelete: false, arguments);
        _channel.QueueBind(_options.SolicitudesQueueName, _options.ExchangeName, "marketplace.v3.reserva.solicitada");
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

public sealed class MarketplaceReservaSolicitadaV3
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid CorrelationId { get; set; }
    public string Source { get; set; } = string.Empty;
    public MarketplaceReservaPayloadV3 Payload { get; set; } = new();
}

public sealed class MarketplaceReservaPayloadV3
{
    public Guid ClienteGuid { get; set; }
    public Guid AtraccionGuid { get; set; }
    public Guid HorarioGuid { get; set; }
    public List<MarketplaceReservaLineaV3> Lineas { get; set; } = [];
    public int ExpiracionMinutos { get; set; } = 15;
    public decimal PorcentajeIva { get; set; }
}

public sealed class MarketplaceReservaLineaV3
{
    public Guid TicketGuid { get; set; }
    public int Cantidad { get; set; }
}
