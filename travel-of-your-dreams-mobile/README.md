# Travel of Your Dreams Mobile

Marketplace movil para clientes internos de TravelDreams. La aplicacion consume el API Gateway existente:

- REST V1 para autenticacion.
- GraphQL V3 para catalogo, reservas, pagos y facturas.
- RabbitMQ permanece detras del gateway y nunca se conecta directamente desde el movil.

## Configuracion

1. Copiar `.env.example` como `.env`.
2. Mantener el gateway desplegado o cambiar la URL por el gateway local:

```env
EXPO_PUBLIC_API_GATEWAY_URL=https://api-gateway-travel-dreams.onrender.com
```

Para Android Emulator con gateway local se debe usar `http://10.0.2.2:5100`. En un telefono fisico se debe usar la IP LAN del equipo, no `localhost`.

## Ejecucion

```powershell
npm install
npm run start
```

Luego se puede abrir con Expo Go escaneando el QR, o ejecutar:

```powershell
npm run android
npm run web
```

## Alcance

- Login de clientes registrados.
- Catalogo y detalle de atracciones.
- Seleccion de horario y tickets.
- Reserva asincronica con seguimiento por `correlationId`.
- Pago simulado y emision de factura.
- Historial de reservas y facturas.
- Perfil y cierre de sesion.

Los endpoints Booking V2 y la administracion web no forman parte de esta aplicacion.
