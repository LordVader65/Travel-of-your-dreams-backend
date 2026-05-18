# Arquitectura de Descomposición a Microservicios: Sistema de Atracciones

## 1. Visión general de la descomposición

El sistema actual de reservas de atracciones turísticas fue construido bajo una arquitectura por capas, separando responsabilidades en proyectos .Api, .Business, .DataManagement y .DataAccess. Esta arquitectura permitió organizar correctamente la lógica de negocio, persistencia, servicios y exposición HTTP.

Sin embargo, desde el punto de vista de microservicios, el sistema sigue concentrando varios dominios dentro de una misma solución y una misma base de datos: identidad, atracciones, reservas, pagos, facturación, clientes, auditoría y catálogos. Por ello, la nueva propuesta consiste en descomponer funcionalmente el sistema actual en microservicios independientes, respetando la lógica de negocio ya definida y los procesos transaccionales implementados.

El objetivo no es modificar el monolito existente, sino usarlo como referencia funcional y técnica para construir nuevos microservicios dentro del mismo repositorio raíz. Cada microservicio tendrá su propia arquitectura limpia, su propia base de datos, sus propias migraciones, su propio despliegue y sus propios límites de responsabilidad.

La separación se realizará por bounded contexts, es decir, por dominios de negocio claramente delimitados. El documento original plantea que un bounded context define límites explícitos donde un modelo de dominio es válido, y recomienda evitar integraciones directas por base de datos entre servicios autónomos.

## 2. Principios de la nueva arquitectura

La arquitectura final se basará en los siguientes principios:

- Principio:	Aplicación en el sistema
- Autonomía:	Cada microservicio tiene su propia base de datos y despliegue.
- Bajo acoplamiento:	No existirán FK cruzadas entre bases de datos de distintos microservicios.
- Alta cohesión:	Cada microservicio agrupa datos y reglas que pertenecen al mismo dominio.
- Comunicación explícita:	Los servicios se comunican por REST, gRPC y eventos, no por joins entre bases.
- Consistencia local:	Cada microservicio mantiene transacciones dentro de su propia base.
- Consistencia eventual:	Los cambios entre servicios se sincronizan mediante eventos cuando no requieren respuesta inmediata.
- Seguridad distribuida:	Identidad emite tokens y los demás servicios validan permisos.
- Trazabilidad:	Todas las solicitudes y eventos deben propagar X-Correlation-ID.
- Idempotencia:	Operaciones críticas como crear reserva, pagar o cancelar deben soportar Idempotency-Key.

## 3. Ecosistema general de microservicios

Booking externo / Frontend / Clientes
        ↓ REST/HTTPS
TravelDreams.ApiGateway
        ↓ REST/gRPC interno
Microservicios
        ↓ Eventos
RabbitMQ - bus de eventos generalmente para auditoria, pero puede servir para otros modulos
        


Los componentes principales serán:
gateway/
└── TravelDreams.ApiGateway/

services/
├── ms-identidad/
├── ms-atracciones/
├── ms-reservas/
├── ms-facturacion/
└── ms-auditoria/        ← opcional/recomendado

building-blocks/
├── TravelDreams.SharedKernel/
├── TravelDreams.EventBus/
└── TravelDreams.Observability/

database/
├── microservices/
└── scripts/

docs/
├── contratos/
├── arquitectura/
├── migracion/
└── seguridad/

## 4. API Gateway

El API Gateway será un proyecto independiente y desplegable. Su función principal será actuar como punto de entrada único para clientes externos, Booking y futuros frontends.

El contrato para integracion del Booking define los siguientes endpoints que deben estar, pero al final se los implementara dado que algunos no coinciden con los desarrollados actualmente, pero hay que tenerlo en cuenta.

- Atracciones públicas
GET /api/v1/atracciones
GET /api/v1/atracciones/{guid}
GET /api/v1/atracciones/{guid}/tickets
GET /api/v1/atracciones/{guid}/horarios-disponibles
GET /api/v1/atracciones/filtros
GET /api/v1/tickets/{guid}/horarios

- Reservas públicas
POST /api/v1/reservas
GET  /api/v1/reservas
GET  /api/v1/reservas/{guid}
POST /api/v1/reservas/{guid}/confirmar-pago

- Reseñas públicas
GET  /api/v1/resenias
POST /api/v1/resenias

RESPONSABILIDAD EL API GATEWAY
- Recibir solicitudes externas.
- Validar JWT de clientes o JWT de servicio externo.
- Enrutar peticiones al microservicio correspondiente.
- Centralizar CORS.
- Propagar X-Correlation-ID.
- Aplicar rate limiting.
- Ocultar la estructura interna de microservicios.
- Unificar el contrato público de Booking.

No debe encargarse de:
- Crear reservas.
- Descontar cupos.
- Confirmar pagos.
- Generar facturas.
- Ejecutar reglas de negocio complejas.
- Acceder directamente a bases de datos.

Para ASP.NET se utilizara ARP, porque permite enrutar sin mezclar lógica de negocio.

## 5. Catálogo de microservicios
### NOTA: Cada microservicio tiene que repsetar la logica y procesos implementada en el proyecto monolito, pero puede variar en algunas cosas debido a la separacion 

- 5.1. ms-identidad

Responsabilidad: Gestiona credenciales, usuarios, roles, autenticación y emisión de tokens JWT.

El documento base define ms-identidad como responsable de la gestión de credenciales, autenticación y autorización base.

Base de datos: ms_identidad_db

Tablas:
usuario
roles
usuarioxroles

Reglas principales
- Un usuario puede tener uno o varios roles.
- Las contraseñas se guardan con hash seguro.
- El microservicio emite JWT.
- Los demás microservicios validan el JWT, pero no consultan directamente la tabla usuario.
- El usu_guid se utiliza como identificador público en claims.

Arquitectura interna
TravelDreams.MsIdentidad.Api
TravelDreams.MsIdentidad.Business
TravelDreams.MsIdentidad.DataManagement
TravelDreams.MsIdentidad.DataAccess

- Endpoints principales
POST /api/v1/auth/login
POST /api/v1/auth/register
GET  /api/v1/auth/me

- Comunicación
Publica eventos como:
identidad.usuario.registrado
identidad.usuario.actualizado
identidad.usuario.inactivado

- 5.2. ms-atracciones

Responsabilidad: Gestiona el catálogo turístico, atracciones, tickets, horarios, cupos, idiomas, categorías, inclusiones, imágenes y reseñas.

Base de datos: ms_atracciones_db

Tablas:
destino
categoria
idioma
incluye
imagen
atraccion
ticket
horario
categoria_atraccion
idioma_atraccion
imagen_atraccion
atraccion_incluye
resenia

Reglas principales
- Una atracción puede tener varios tickets.
- Una atracción puede tener varios horarios.
- El cupo disponible pertenece al horario.
- Los cupos se reservan o liberan desde este microservicio.
- Un ticket debe pertenecer a la atracción del horario seleccionado.
- Una reseña debe estar asociada a una reserva válida/usada.

Arquitectura interna
TravelDreams.MsAtracciones.Api
TravelDreams.MsAtracciones.Business
TravelDreams.MsAtracciones.DataManagement
TravelDreams.MsAtracciones.DataAccess

Endpoints públicos

Estos endpoints soportan el contrato Booking:

GET /api/v1/atracciones
GET /api/v1/atracciones/{guid}
GET /api/v1/atracciones/{guid}/tickets
GET /api/v1/atracciones/{guid}/horarios-disponibles
GET /api/v1/atracciones/filtros
GET /api/v1/tickets/{guid}/horarios
GET /api/v1/resenias
POST /api/v1/resenias

Endpoints administrativos
GET    /api/v1/admin/atracciones
POST   /api/v1/admin/atracciones
PUT    /api/v1/admin/atracciones/{guid}
DELETE /api/v1/admin/atracciones/{guid}

GET/POST/PUT/DELETE /api/v1/admin/tickets
GET/POST/PUT/DELETE /api/v1/admin/horarios
GET/POST/PUT/DELETE /api/v1/admin/destinos
GET/POST/PUT/DELETE /api/v1/admin/categorias

- Comunicación gRPC
ms-atracciones debe exponer servicios gRPC para ms-reservas, para seguir la logica de negocio aplicada

- 5.3. ms-reservas
Responsabilidad: Gestiona el ciclo de vida de reservas: creación, consulta, cancelación, expiración, historial de estados y relación con clientes.

En esta primera separación, clientes se mantiene dentro de ms-reservas, porque el cliente está directamente relacionado con la reserva, se debe  cliente invitado, consulta de “mis reservas” y datos operativos. A futuro podría extraerse como ms-clientes, pero no se considera necesario en la primera descomposición.

Base de datos

ms_reservas_db
Tablas
clientes
reservas
reserva_detalle
reserva_estado_historial

Reglas principales (pueden faltar, pero considera lo que tenia el monolito para la gestio interna)
- Una reserva pertenece a un cliente.
- Una reserva puede tener varias líneas de detalle.
- La reserva se crea únicamente si existe disponibilidad.
- La reserva inicia como PENDIENTE.
- La reserva pendiente tiene fecha de expiración.
- Si expira, debe liberar cupos.
- Si se cancela válidamente, debe liberar cupos.
- Una reserva pagada no debe cancelarse bajo la misma regla que una pendiente.
- El historial de estado debe registrar cambios relevantes.

Arquitectura interna
TravelDreams.MsReservas.Api
TravelDreams.MsReservas.Business
TravelDreams.MsReservas.DataManagement
TravelDreams.MsReservas.DataAccess

- Endpoints públicos
GET  /api/v1/reservas
POST /api/v1/reservas
GET  /api/v1/reservas/{guid}
PUT  /api/v1/reservas/{guid}/cancelar

El contrato de Booking exige POST /reservas, GET /reservas, GET /reservas/{guid} y PUT /reservas/{guid}/cancelar, con 204 para cancelación correcta.

- Endpoints administrativos
GET /api/v1/admin/reservas
GET /api/v1/admin/reservas/{guid}
PUT /api/v1/admin/reservas/{guid}/estado

Comunicación con ms-atracciones

ms-reservas debe llamar a ms-atracciones por gRPC cuando necesite una validación inmediata, cuando:
- Se crea una reserva
- Se cancela una reserva
- Expira una reserva

- 5.4. ms-facturacion
Responsabilidad: Gestiona pagos, datos de facturación, emisión de facturas y consulta de comprobantes.

Aunque inicialmente se listaron solo facturas y datos_facturacion, se recomienda incluir también pagos en este microservicio, porque forma parte del proceso financiero y actualmente está acoplado al flujo pago → factura.

Base de datos: ms_facturacion_db

Tablas
pagos
facturas
datos_facturacion

Reglas principales (pueden faltar, pero considera lo que tenia el monolito para la gestio interna)
- Una reserva solo puede pagarse si está pendiente y vigente.
- El pago debe tener referencia única.
- El monto pagado debe coincidir con el total de la reserva.
- La factura solo se genera después de un pago válido.
- Los datos de facturación pueden ser distintos al cliente dueño de la reserva.
- Una factura referencia la reserva mediante rev_guid débil, no por FK cruzada.

Arquitectura interna
TravelDreams.MsFacturacion.Api
TravelDreams.MsFacturacion.Business
TravelDreams.MsFacturacion.DataManagement
TravelDreams.MsFacturacion.DataAccess

Endpoints públicos
POST /api/v1/reservas/{guid}/confirmar-pago
GET  /api/v1/facturas/mis-facturas

El contrato define POST /reservas/{guid}/confirmar-pago con respuesta 201 y una factura generada, además de GET /facturas/mis-facturas.

Comunicación
Opción síncrona para cumplir Booking
POST /reservas/{guid}/confirmar-pago
    → ms-facturacion
    → gRPC ms-reservas para consultar datos de la reserva
    → registra pago
    → genera factura
    → responde 201 con factura

- 5.5. ms-auditoria
Responsabilidad: Registrar eventos críticos de forma transversal e inmutable.

Este microservicio es opcional para una primera versión, pero recomendado si se implementa bus de eventos. El documento original propone ms-auditoria como servicio que consume eventos del bus y los almacena como log transversal.

Base de datos: ms_auditoria_db
Tablas sugeridas

auditoria_log
eventos_procesados
Comunicación

No debe ser llamado directamente por otros servicios en la mayoría de casos. Debe consumir eventos

## 6. API pública para Booking

Booking externo consumirá únicamente el API Gateway. Los microservicios no se exponen directamente.

Endpoints públicos

Atracciones públicas
GET /api/v1/atracciones
GET /api/v1/atracciones/{guid}
GET /api/v1/atracciones/{guid}/tickets
GET /api/v1/atracciones/{guid}/horarios-disponibles
GET /api/v1/atracciones/filtros
GET /api/v1/tickets/{guid}/horarios
Reservas públicas

POST /api/v1/reservas
GET  /api/v1/reservas
GET  /api/v1/reservas/{guid}
POST /api/v1/reservas/{guid}/confirmar-pago

Reseñas públicas
GET  /api/v1/resenias
POST /api/v1/resenias


## 7. Comunicación entre microservicios
7.1. REST

Se usa principalmente para:

- Comunicación externa desde API Gateway.
- Endpoints públicos.
- Endpoints administrativos.
7.2. gRPC

Se usa para comunicación interna síncrona entre microservicios cuando se requiere respuesta inmediata.

7.3. Bus de eventos

Se usa para comunicación asíncrona.

El bus no es lo mismo que el API Gateway. El Gateway es un proyecto desplegable que recibe tráfico externo; el bus es infraestructura de mensajería, en este caso RabbitMQ con topic exchanges y patrón Outbox para evitar el problema de guardar en BD y publicar evento por separado.

## 8. Diseño del Bus de Eventos

8.1. Broker recomendado

Como el despliegue será en Render, usaremos RabbitMQ

8.2. Librería compartida

Se recomienda crear:

building-blocks/
└── TravelDreams.EventBus/

Esta librería contendrá:

IEventBus
IntegrationEvent
RabbitMqEventBus
EventPublisher
EventConsumer
OutboxEvent
ProcessedEvent

La librería no reemplaza el broker. Solo contiene el código que permite conectarse al broker.

8.3. Outbox Pattern

Cada microservicio que publique eventos debe tener una tabla local:

outbox_events

Flujo:

1. El microservicio ejecuta su transacción local.
2. Guarda los datos de negocio.
3. Guarda el evento en outbox_events dentro de la misma transacción.
4. Un worker lee outbox_events.
5. El worker publica al broker.
6. Marca el evento como publicado.

## 9. Manejo de transacciones y eliminación de FK cruzadas
9.1. Regla principal

Dentro de una misma base de datos sí se pueden mantener FK. Entre bases de datos de microservicios no deben existir FK cruzadas.

9.2. Referencias débiles por GUID

Cuando un microservicio necesita guardar referencia a datos de otro servicio, guarda el GUID, pero no una FK física.

9.3. Reemplazo de funciones SQL transversales

En el monolito, funciones como fn_crear_reserva podían validar cliente, horario, ticket, atracción y cupos en una sola base. En microservicios, esa lógica se divide por dueño del dato.

9.4. Compensación

Si ms-atracciones reserva cupos, pero ms-reservas falla al guardar la reserva, ms-reservas debe llamar:

ReleaseAvailability

para liberar los cupos.

## 10. Arquitectura por capas en cada microservicio

Cada microservicio mantendrá la arquitectura limpia que ya se venía trabajando:

.Api
.Business
.DataManagement
.DataAccess

La guía de implementación base describe esta arquitectura con DataAccess, DataManagement, Business y Api como proyectos separados.

10.1. Api

Responsable de:

- Controllers
- Autenticación
- Autorización
- Swagger
- Versionamiento
- Middleware de errores
- CORS

10.2. Business

Responsable de:

- Reglas de negocio
- Validaciones
- Orquestación de casos de uso
- Excepciones funcionales
- Mapeo DTO ↔ DataModel

La capa Business interpreta requerimientos, aplica reglas, orquesta datos y controla casos de uso.

10.3. DataManagement

Responsable de:

- UnitOfWork
- DataServices
- Transacciones locales
- Coordinación de repositorios
- Modelos de datos

La guía define UnitOfWork como el componente que agrupa repositorios, confirma cambios y puede manejar transacciones.

10.4. DataAccess

Responsable de:

- DbContext
- Entities
- Configurations
- Repositories
- QueryRepositories
- Migrations
- Scripts SQL locales

## 11. Migraciones y bases PostgreSQL en Azure

Cada microservicio tendrá su propia base de datos en Azure PostgreSQL y su propio DbContext.

11.1. Bases
ms_identidad_db
ms_atracciones_db
ms_reservas_db
ms_facturacion_db
ms_auditoria_db 

11.2. Migraciones

Cada proyecto .DataAccess tendrá su carpeta Migrations.

Ejemplo:

services/ms-reservas/
└── TravelDreams.MsReservas.DataAccess/
    ├── Context/
    │   └── ReservasDbContext.cs
    ├── Migrations/
    ├── Entities/
    ├── Configurations/
    └── Scripts/

11.3. Funciones SQL en migraciones

Las funciones o procedimientos locales de cada microservicio pueden incluirse dentro de migraciones.

Las funciones que antes dependían de tablas de varios dominios deben reestructurarse para que solo operen sobre tablas del microservicio dueño. Si necesitan información externa, deben usar gRPC antes de la transacción local o eventos después de la transacción.

## 12. Despliegue en Render

Cada microservicio será un servicio independiente en Render.

12.1. Servicios Web
TravelDreams.ApiGateway
TravelDreams.MsIdentidad.Api
TravelDreams.MsAtracciones.Api
TravelDreams.MsReservas.Api
TravelDreams.MsFacturacion.Api

Nos recomienda usar workers para:
- Publicar eventos Outbox.
- Consumir eventos del bus.
- Expirar reservas pendientes.
- Generar facturas por eventos.
- Registrar auditoría.

Creando
TravelDreams.MsReservas.Worker
TravelDreams.MsFacturacion.Worker
TravelDreams.MsAuditoria.Worker

Sin embargo, si existe la posibilidad de implementarlos dentro de algunas de las capas ya definidas implementalo.

12.4. Variables de entorno

Cada servicio debe tener sus propias variables:

ConnectionStrings__DefaultConnection
JwtSettings__Issuer
JwtSettings__Audience
JwtSettings__PublicKey
RabbitMq__Host
RabbitMq__User
RabbitMq__Password
Services__AtraccionesGrpcUrl
Services__ReservasGrpcUrl

