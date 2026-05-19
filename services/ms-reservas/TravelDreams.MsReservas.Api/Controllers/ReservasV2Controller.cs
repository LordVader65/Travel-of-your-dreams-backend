using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsReservas.Business.DTOs;
using TravelDreams.MsReservas.Business.Interfaces;

namespace TravelDreams.MsReservas.Api.Controllers;

[ApiController]
[Route("api/v2/reservas")]
public sealed class ReservasV2Controller : ControllerBase
{
    private readonly IReservasService _reservas;
    private readonly IClientesService _clientes;

    public ReservasV2Controller(IReservasService reservas, IClientesService clientes)
    {
        _reservas = reservas;
        _clientes = clientes;
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearReservaV2Request request, CancellationToken ct)
    {
        if (request.AtGuid == Guid.Empty) return BadRequest(Error(400, "Body invalido", "El campo 'at_guid' es obligatorio."));
        if (request.HorGuid == Guid.Empty) return BadRequest(Error(400, "Body invalido", "El campo 'hor_guid' es obligatorio."));
        if (request.Lineas.Count == 0) return BadRequest(Error(400, "Body invalido", "Debe enviar al menos una linea."));
        if (request.Lineas.Any(x => x.TckGuid == Guid.Empty || x.Cantidad < 1)) return BadRequest(Error(400, "Body invalido", "Cada linea debe tener tck_guid y cantidad mayor o igual a 1."));

        var isBookingIntegration = IsBookingIntegration();
        if (isBookingIntegration && request.ClienteInvitado is null)
        {
            return BadRequest(Error(400, "Body invalido", "cliente_invitado es obligatorio para reservas creadas por Booking."));
        }

        Guid? clienteGuid = null;
        ClienteInvitadoV2Request? clienteInvitado = request.ClienteInvitado;
        if (!isBookingIntegration && TryGetUserGuid(out _))
        {
            clienteGuid = await ResolveClienteGuidFromHeadersAsync(ct);
            clienteInvitado = null;
        }

        if (!clienteGuid.HasValue && clienteInvitado is null)
        {
            return BadRequest(Error(400, "Body invalido", "cliente_invitado es obligatorio cuando no se envia JWT de cliente."));
        }

        var model = new CrearReservaRequest
        {
            ClienteGuid = clienteGuid,
            ClienteInvitado = clienteInvitado?.ToClienteRequest(),
            AtraccionGuid = request.AtGuid,
            HorarioGuid = request.HorGuid,
            OrigenCanal = isBookingIntegration ? "BOOKING" : request.OrigenCanal ?? "BOOKING",
            PorcentajeIva = 12,
            ExpiracionMinutos = 30,
            Lineas = request.Lineas.Select(x => new CrearReservaLineaRequest
            {
                TicketGuid = x.TckGuid,
                Cantidad = x.Cantidad
            }).ToList()
        };

        var data = await _reservas.CrearAsync(model, ct);
        return Created(string.Empty, new { status = 201, message = "Operacion exitosa", data = ToV2Detail(data) });
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        if (page < 1) return BadRequest(Error(400, "Parametro invalido", "El campo 'page' debe ser mayor o igual a 1."));
        if (limit < 1 || limit > 50) return BadRequest(Error(400, "Parametro invalido", "El campo 'limit' debe estar entre 1 y 50."));

        IReadOnlyList<ReservaResponse> reservas;
        if (IsBookingIntegration())
        {
            reservas = await _reservas.ListarPorCanalAsync("BOOKING", null, ct);
        }
        else
        {
            var clienteGuid = await ResolveClienteGuidFromHeadersAsync(ct);
            if (!IsAdmin() && !clienteGuid.HasValue)
            {
                return Unauthorized(Error(401, "Token ausente o invalido", "No se pudo resolver el cliente autenticado."));
            }

            reservas = await _reservas.ListarAsync(IsAdmin() ? null : clienteGuid, null, ct);
        }

        var total = reservas.Count;
        var data = reservas
            .OrderByDescending(x => x.FechaReservaUtc)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(ToV2Summary)
            .ToList();

        return Ok(new
        {
            status = 200,
            message = "Operacion exitosa",
            data,
            pagination = new
            {
                page,
                limit,
                total,
                total_pages = (int)Math.Ceiling(total / (double)limit)
            }
        });
    }

    [HttpGet("{guid:guid}")]
    public async Task<IActionResult> Obtener(Guid guid, CancellationToken ct)
    {
        var data = await _reservas.ObtenerAsync(guid, ct);
        if (data is null) return NotFound(new { status = 404, error = "Reserva no encontrada." });

        if (!await CanAccessReservaAsync(data, ct)) return StatusCode(403, new { status = 403, error = "La reserva no pertenece al cliente autenticado." });

        return Ok(new { status = 200, message = "Operacion exitosa", data = ToV2Detail(data) });
    }

    private static object ToV2Summary(ReservaResponse reserva) => new
    {
        rev_guid = reserva.Guid,
        rev_codigo = reserva.Codigo,
        hor_fecha = reserva.HorFecha,
        hor_hora_inicio = FormatTime(reserva.HorHoraInicio),
        atraccion_nombre = reserva.AtraccionNombre,
        rev_total = reserva.Total,
        moneda = reserva.Moneda,
        rev_estado = reserva.Estado,
        _links = new { self = $"/api/v2/reservas/{reserva.Guid}" }
    };

    private static object ToV2Detail(ReservaResponse reserva) => new
    {
        rev_guid = reserva.Guid,
        rev_codigo = reserva.Codigo,
        hor_fecha = reserva.HorFecha,
        hor_hora_inicio = FormatTime(reserva.HorHoraInicio),
        hor_hora_fin = FormatTime(reserva.HorHoraFin),
        atraccion_nombre = reserva.AtraccionNombre,
        rev_subtotal = reserva.Subtotal,
        rev_valor_iva = reserva.ValorIva,
        rev_total = reserva.Total,
        moneda = reserva.Moneda,
        rev_estado = reserva.Estado,
        rev_fecha_reserva_utc = reserva.FechaReservaUtc,
        detalle = reserva.Detalles.Select(x => new
        {
            tck_tipo_participante = x.TipoParticipante,
            cantidad = x.Cantidad,
            precio_unit = x.PrecioUnitario,
            subtotal = x.Subtotal
        }).ToList(),
        _links = new
        {
            self = $"/api/v2/reservas/{reserva.Guid}",
            confirmar_pago = $"/api/v2/reservas/{reserva.Guid}/pagos/confirmacion"
        }
    };

    private async Task<Guid?> ResolveClienteGuidFromHeadersAsync(CancellationToken ct)
    {
        if (!TryGetUserGuid(out var usuarioGuid)) return null;
        var cliente = await _clientes.ObtenerPorUsuarioGuidAsync(usuarioGuid, ct);
        return cliente?.Guid;
    }

    private async Task<bool> CanAccessReservaAsync(ReservaResponse reserva, CancellationToken ct)
    {
        if (IsAdmin()) return true;
        if (IsBookingIntegration())
        {
            return string.Equals(reserva.OrigenCanal, "BOOKING", StringComparison.OrdinalIgnoreCase);
        }

        var clienteGuid = await ResolveClienteGuidFromHeadersAsync(ct);
        return clienteGuid.HasValue && clienteGuid.Value == reserva.ClienteGuid;
    }

    private bool TryGetUserGuid(out Guid usuarioGuid)
    {
        usuarioGuid = Guid.Empty;
        return Request.Headers.TryGetValue("X-User-Guid", out var value)
            && Guid.TryParse(value.ToString(), out usuarioGuid);
    }

    private bool IsAdmin() =>
        Request.Headers.TryGetValue("X-Roles", out var roles)
        && roles.ToString().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(x => x.Equals("ADMIN", StringComparison.OrdinalIgnoreCase));

    private bool IsBookingIntegration() =>
        Request.Headers.TryGetValue("X-Roles", out var roles)
        && roles.ToString().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(x => x.Equals("BOOKING_INTEGRATION", StringComparison.OrdinalIgnoreCase));

    private object Error(int status, string error, string detail) => new
    {
        status,
        error,
        details = new[] { detail },
        timestamp = DateTime.UtcNow,
        path = Request.Path.Value
    };

    private static string? FormatTime(TimeOnly? time) => time?.ToString("HH:mm");
}

public sealed class CrearReservaV2Request
{
    [JsonPropertyName("at_guid")]
    public Guid AtGuid { get; set; }

    [JsonPropertyName("hor_guid")]
    public Guid HorGuid { get; set; }

    [JsonPropertyName("lineas")]
    public IReadOnlyList<CrearReservaLineaV2Request> Lineas { get; set; } = [];

    [JsonPropertyName("origen_canal")]
    public string? OrigenCanal { get; set; }

    [JsonPropertyName("cliente_invitado")]
    public ClienteInvitadoV2Request? ClienteInvitado { get; set; }
}

public sealed class CrearReservaLineaV2Request
{
    [JsonPropertyName("tck_guid")]
    public Guid TckGuid { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; }
}

public sealed class ClienteInvitadoV2Request
{
    [JsonPropertyName("tipo_identificacion")]
    public string TipoIdentificacion { get; set; } = "OTRO";

    [JsonPropertyName("numero_identificacion")]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [JsonPropertyName("nombres")]
    public string? Nombres { get; set; }

    [JsonPropertyName("apellidos")]
    public string? Apellidos { get; set; }

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("direccion")]
    public string? Direccion { get; set; }

    public ClienteRequest ToClienteRequest() => new()
    {
        TipoIdentificacion = TipoIdentificacion,
        NumeroIdentificacion = NumeroIdentificacion,
        Nombres = Nombres,
        Apellidos = Apellidos,
        Correo = Correo,
        Telefono = Telefono,
        Direccion = Direccion
    };
}
