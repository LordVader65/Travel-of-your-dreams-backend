# TravelDreams API Gateway

Gateway HTTP para enrutar llamadas externas hacia los microservicios de TravelDreams.

## Ejecucion local

Levantar primero los microservicios:

```powershell
dotnet run --project "services/ms-identidad/TravelDreams.MsIdentidad.Api/TravelDreams.MsIdentidad.Api.csproj" --urls "http://localhost:5101"
dotnet run --project "services/ms-atracciones/TravelDreams.MsAtracciones.Api/TravelDreams.MsAtracciones.Api.csproj" --urls "http://localhost:5102"
dotnet run --project "services/ms-reservas/TravelDreams.MsReservas.Api/TravelDreams.MsReservas.Api.csproj" --urls "http://localhost:5103"
dotnet run --project "services/ms-facturacion/TravelDreams.MsFacturacion.Api/TravelDreams.MsFacturacion.Api.csproj" --urls "http://localhost:5104"
dotnet run --project "services/ms-auditoria/TravelDreams.MsAuditoria.Api/TravelDreams.MsAuditoria.Api.csproj" --urls "http://localhost:5105"
```

Levantar gateway:

```powershell
dotnet run --project "gateway/TravelDreams.ApiGateway/TravelDreams.ApiGateway.csproj" --urls "http://localhost:5100"
```

## Rutas principales

| Prefijo | Destino |
| --- | --- |
| `/api/v1/auth` | ms-identidad |
| `/api/v1/admin/auth` | ms-identidad |
| `/api/v1/admin/usuarios` | ms-identidad |
| `/api/v1/admin/roles` | ms-identidad |
| `/api/v1/atracciones` | ms-atracciones |
| `/api/v1/tickets` | ms-atracciones |
| `/api/v1/resenias` | ms-atracciones |
| `/api/v1/admin/atracciones` | ms-atracciones |
| `/api/v1/admin/catalogos` | ms-atracciones |
| `/api/v1/admin/horarios` | ms-atracciones |
| `/api/v1/admin/tickets` | ms-atracciones |
| `/api/v1/admin/resenias` | ms-atracciones |
| `/api/v1/reservas` | ms-reservas, salvo `/confirmar-pago` |
| `/api/v1/admin/reservas` | ms-reservas |
| `/api/v1/admin/clientes` | ms-reservas, salvo `/datos-facturacion` |
| `/api/v1/me` | ms-reservas, salvo `/password` y `/datos-facturacion` |
| `/api/v1/reservas/{guid}/confirmar-pago` | ms-facturacion |
| `/api/v1/reservas/{guid}/confirmar-pago-receptor` | ms-facturacion |
| `/api/v1/me/datos-facturacion` | ms-facturacion |
| `/api/v1/pagos` | ms-facturacion |
| `/api/v1/facturas` | ms-facturacion |
| `/api/v1/admin/pagos` | ms-facturacion |
| `/api/v1/admin/facturas` | ms-facturacion |
| `/api/v1/admin/clientes/{guid}/datos-facturacion` | ms-facturacion |
| `/api/v1/admin/auditoria` | ms-auditoria |
| `/api/v1/integrations/booking/token` | API Gateway |
| `/api/v1/booking/reservas/{guid}` | ms-reservas |
| `/api/v1/booking/reservas/{guid}/cancelar` | ms-reservas |
| `/api/v1/booking/facturas?reservaGuid=...` | ms-facturacion |

## Variables de entorno

```text
Services__IdentidadUrl
Services__AtraccionesUrl
Services__ReservasUrl
Services__FacturacionUrl
Services__AuditoriaUrl
Gateway__EnableAuditLogging
JwtSettings__Issuer
JwtSettings__Audience
JwtSettings__SecretKey
BookingAuth__ClientId
BookingAuth__ClientSecret
BookingAuth__Issuer
BookingAuth__Audience
BookingAuth__SecretKey
BookingAuth__ExpirationMinutes
```

En Render, configurar estas variables con las URL publicas o privadas de cada servicio desplegado.

`JwtSettings__Issuer`, `JwtSettings__Audience` y `JwtSettings__SecretKey` deben tener exactamente los mismos valores que usa `ms-identidad`, porque el gateway valida los JWT emitidos por ese microservicio.

## Seguridad

El gateway valida JWT antes de reenviar rutas privadas. Si el token es valido, agrega estos headers hacia el microservicio destino:

```text
X-User-Guid
X-User
X-Roles
X-Correlation-ID
```

Los endpoints publicos de atracciones quedan disponibles sin token. Las rutas admin requieren rol `ADMIN`. Las rutas cliente requieren rol `CLIENTE` o `ADMIN`.

Las rutas de cliente pueden derivar el cliente desde el JWT. El gateway valida el token y pasa `X-User-Guid`; `ms-reservas` resuelve ese usuario contra `clientes.usu_guid`. `ms-facturacion` consulta a `ms-reservas` para resolver el mismo dato cuando necesita listar pagos o facturas del cliente autenticado.

## Integracion Booking

Booking obtiene un token de servicio con:

```http
POST /api/v1/integrations/booking/token
```

```json
{
  "clientId": "booking-app",
  "clientSecret": "CAMBIA_ESTE_SECRETO_BOOKING"
}
```

El gateway emite un JWT con rol `BOOKING_INTEGRATION`. Ese token permite crear reservas por canal `BOOKING`, confirmar pago por receptor, consultar reservas Booking por GUID, cancelar reservas Booking y consultar facturas por `reservaGuid`.
