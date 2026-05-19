using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TravelDreams.MsFacturacion.Business.DTOs;
using TravelDreams.MsFacturacion.Business.Interfaces;

namespace TravelDreams.MsFacturacion.Api.Controllers;

[ApiController]
[Route("api/v2/reservas/{guid:guid}/pagos")]
public sealed class PagosV2Controller : ControllerBase
{
    private readonly IPagoService _pagos;

    public PagosV2Controller(IPagoService pagos)
    {
        _pagos = pagos;
    }

    [HttpPost("confirmacion")]
    public async Task<IActionResult> ConfirmarPago(Guid guid, ConfirmarPagoV2Request request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.NombreReceptor))
        {
            return BadRequest(Error(400, "Body invalido", "nombre_receptor es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(request.CorreoReceptor) || !request.CorreoReceptor.Contains('@'))
        {
            return BadRequest(Error(400, "Body invalido", "correo_receptor es obligatorio y debe ser valido."));
        }

        var factura = await _pagos.ConfirmarPagoConReceptorAsync(guid, new ConfirmarPagoReceptorRequest
        {
            NombreReceptor = request.NombreReceptor,
            ApellidoReceptor = request.ApellidoReceptor,
            CorreoReceptor = request.CorreoReceptor,
            TelefonoReceptor = request.TelefonoReceptor,
            Observacion = request.Observacion,
            OrigenCanal = "BOOKING"
        }, ct);

        return Created(string.Empty, new
        {
            status = 201,
            message = "Operacion exitosa",
            data = new
            {
                fac_guid = factura.Guid,
                fac_numero = factura.Numero,
                rev_codigo = factura.ReservaGuid.ToString(),
                total = factura.Total,
                moneda = factura.Moneda,
                fecha_emision = factura.FechaEmisionUtc,
                estado = factura.Estado,
                nombre_receptor = request.NombreReceptor,
                correo_receptor = request.CorreoReceptor
            }
        });
    }

    private object Error(int status, string error, string detail) => new
    {
        status,
        error,
        details = new[] { detail },
        timestamp = DateTime.UtcNow,
        path = Request.Path.Value
    };
}

public sealed class ConfirmarPagoV2Request
{
    [JsonPropertyName("nombre_receptor")]
    public string NombreReceptor { get; set; } = string.Empty;

    [JsonPropertyName("apellido_receptor")]
    public string? ApellidoReceptor { get; set; }

    [JsonPropertyName("correo_receptor")]
    public string CorreoReceptor { get; set; } = string.Empty;

    [JsonPropertyName("telefono_receptor")]
    public string? TelefonoReceptor { get; set; }

    [JsonPropertyName("observacion")]
    public string? Observacion { get; set; }
}
