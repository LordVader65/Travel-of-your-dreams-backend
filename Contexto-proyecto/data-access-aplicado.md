# Complemento específico para AtraccionesTuristicas.Backend.LA.DataAccess

## Contexto real del proyecto

Este documento se usa como referencia conceptual y técnica para implementar la capa:

AtraccionesTuristicas.Backend.LA.DataAccess

La guía original estaba basada en un ejemplo de clientes con SQL Server. Para este proyecto NO se debe copiar literalmente ese ejemplo. Se debe adaptar al sistema de reservas de atracciones turísticas usando:

- .NET / ASP.NET Core según la versión actual de la solución.
- TargetFramework del proyecto: net10.0.
- Entity Framework Core.
- Npgsql para PostgreSQL.
- PostgreSQL desplegado en Azure.
- La API está pensada para desplegarse posteriormente en Render.
- La cadena de conexión y secretos se obtienen desde variables de entorno.
- appsettings.json solo mantiene claves vacías o valores no sensibles.
- Migraciones EF Core dentro del proyecto DataAccess.
- Modelo de datos sin esquema explícito.
- Tablas en el esquema por defecto de PostgreSQL, normalmente public.
- Arquitectura por capas: DataAccess, DataManagement, Business y Api.

## Importante sobre configuración

La capa DataAccess NO debe leer directamente:

- appsettings.json
- .env
- variables de entorno
- configuración de Render
- configuración de Azure

La conexión a base de datos se configura desde el proyecto Api usando inyección de dependencias.

DataAccess solamente debe contener:

- Entidades.
- Configuraciones EF Core.
- DbContext.
- Repositorios.
- Query repositories.
- Scripts SQL.
- Migraciones.

El DbContext debe recibir DbContextOptions<AtraccionesDbContext> por constructor.

Ejemplo correcto:

public AtraccionesDbContext(DbContextOptions<AtraccionesDbContext> options)
    : base(options)
{
}

Ejemplo incorrecto:

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseNpgsql("cadena quemada");
}

## Restricción principal

No inventar estructura de carpetas ni archivos fuera de lo definido.

La estructura válida de la capa DataAccess es:

AtraccionesTuristicas.Backend.LA.DataAccess
│
├── Common
│   ├── PagedResult.cs
│   ├── SortDirection.cs
│   └── DatabaseConstants.cs
│
├── Context
│   └── AtraccionesDbContext.cs
│
├── Entities
│   ├── Identity
│   │   ├── RolEntity.cs
│   │   ├── UsuarioEntity.cs
│   │   └── UsuarioRolEntity.cs
│   │
│   ├── Cliente
│   │   ├── ClienteEntity.cs
│   │   └── DatosFacturacionEntity.cs
│   │
│   ├── Catalogo
│   │   ├── DestinoEntity.cs
│   │   ├── CategoriaEntity.cs
│   │   ├── IdiomaEntity.cs
│   │   ├── IncluyeEntity.cs
│   │   ├── ImagenEntity.cs
│   │   └── AtraccionEntity.cs
│   │
│   ├── CatalogoRelaciones
│   │   ├── CategoriaAtraccionEntity.cs
│   │   ├── IdiomaAtraccionEntity.cs
│   │   ├── ImagenAtraccionEntity.cs
│   │   └── AtraccionIncluyeEntity.cs
│   │
│   ├── Operacion
│   │   ├── TicketEntity.cs
│   │   ├── HorarioEntity.cs
│   │   ├── ReservaEntity.cs
│   │   ├── ReservaDetalleEntity.cs
│   │   ├── ReservaEstadoHistorialEntity.cs
│   │   ├── PagoEntity.cs
│   │   ├── FacturaEntity.cs
│   │   └── ReseniaEntity.cs
│   │
│   └── Auditoria
│       └── AuditoriaLogEntity.cs
│
├── Configurations
│   ├── Identity
│   │   ├── RolConfiguration.cs
│   │   ├── UsuarioConfiguration.cs
│   │   └── UsuarioRolConfiguration.cs
│   │
│   ├── Cliente
│   │   ├── ClienteConfiguration.cs
│   │   └── DatosFacturacionConfiguration.cs
│   │
│   ├── Catalogo
│   │   ├── DestinoConfiguration.cs
│   │   ├── CategoriaConfiguration.cs
│   │   ├── IdiomaConfiguration.cs
│   │   ├── IncluyeConfiguration.cs
│   │   ├── ImagenConfiguration.cs
│   │   └── AtraccionConfiguration.cs
│   │
│   ├── CatalogoRelaciones
│   │   ├── CategoriaAtraccionConfiguration.cs
│   │   ├── IdiomaAtraccionConfiguration.cs
│   │   ├── ImagenAtraccionConfiguration.cs
│   │   └── AtraccionIncluyeConfiguration.cs
│   │
│   ├── Operacion
│   │   ├── TicketConfiguration.cs
│   │   ├── HorarioConfiguration.cs
│   │   ├── ReservaConfiguration.cs
│   │   ├── ReservaDetalleConfiguration.cs
│   │   ├── ReservaEstadoHistorialConfiguration.cs
│   │   ├── PagoConfiguration.cs
│   │   ├── FacturaConfiguration.cs
│   │   └── ReseniaConfiguration.cs
│   │
│   └── Auditoria
│       └── AuditoriaLogConfiguration.cs
│
├── Repositories
│   ├── Interfaces
│   │   ├── IRepositoryBase.cs
│   │   ├── IRolRepository.cs
│   │   ├── IUsuarioRepository.cs
│   │   ├── IUsuarioRolRepository.cs
│   │   ├── IClienteRepository.cs
│   │   ├── IDatosFacturacionRepository.cs
│   │   ├── IDestinoRepository.cs
│   │   ├── ICategoriaRepository.cs
│   │   ├── IIdiomaRepository.cs
│   │   ├── IIncluyeRepository.cs
│   │   ├── IImagenRepository.cs
│   │   ├── IAtraccionRepository.cs
│   │   ├── ICategoriaAtraccionRepository.cs
│   │   ├── IIdiomaAtraccionRepository.cs
│   │   ├── IImagenAtraccionRepository.cs
│   │   ├── IAtraccionIncluyeRepository.cs
│   │   ├── ITicketRepository.cs
│   │   ├── IHorarioRepository.cs
│   │   ├── IReservaRepository.cs
│   │   ├── IReservaDetalleRepository.cs
│   │   ├── IReservaEstadoHistorialRepository.cs
│   │   ├── IPagoRepository.cs
│   │   ├── IFacturaRepository.cs
│   │   ├── IReseniaRepository.cs
│   │   └── IAuditoriaLogRepository.cs
│   │
│   ├── RepositoryBase.cs
│   ├── RolRepository.cs
│   ├── UsuarioRepository.cs
│   ├── UsuarioRolRepository.cs
│   ├── ClienteRepository.cs
│   ├── DatosFacturacionRepository.cs
│   ├── DestinoRepository.cs
│   ├── CategoriaRepository.cs
│   ├── IdiomaRepository.cs
│   ├── IncluyeRepository.cs
│   ├── ImagenRepository.cs
│   ├── AtraccionRepository.cs
│   ├── CategoriaAtraccionRepository.cs
│   ├── IdiomaAtraccionRepository.cs
│   ├── ImagenAtraccionRepository.cs
│   ├── AtraccionIncluyeRepository.cs
│   ├── TicketRepository.cs
│   ├── HorarioRepository.cs
│   ├── ReservaRepository.cs
│   ├── ReservaDetalleRepository.cs
│   ├── ReservaEstadoHistorialRepository.cs
│   ├── PagoRepository.cs
│   ├── FacturaRepository.cs
│   ├── ReseniaRepository.cs
│   └── AuditoriaLogRepository.cs
│
├── Queries
│   ├── Interfaces
│   │   ├── IClienteQueryRepository.cs
│   │   ├── IAtraccionQueryRepository.cs
│   │   ├── IHorarioQueryRepository.cs
│   │   ├── IReservaQueryRepository.cs
│   │   ├── IPagoQueryRepository.cs
│   │   ├── IFacturaQueryRepository.cs
│   │   └── IAuditoriaLogQueryRepository.cs
│   │
│   ├── ClienteQueryRepository.cs
│   ├── AtraccionQueryRepository.cs
│   ├── HorarioQueryRepository.cs
│   ├── ReservaQueryRepository.cs
│   ├── PagoQueryRepository.cs
│   ├── FacturaQueryRepository.cs
│   └── AuditoriaLogQueryRepository.cs
│
├── Scripts
│   ├── Extensions
│   │   └── 001_create_pgcrypto.sql
│   │
│   ├── Functions
│   │   ├── 001_fn_increment_row_version_generic.sql
│   │   ├── 002_fn_crear_reserva.sql
│   │   ├── 003_fn_cancelar_reserva.sql
│   │   ├── 004_fn_expirar_reservas_pendientes.sql
│   │   ├── 005_fn_confirmar_pago.sql
│   │   ├── 006_fn_generar_factura.sql
│   │   └── 007_fn_registrar_auditoria.sql
│   │
│   ├── Triggers
│   │   ├── 001_trg_clientes_row_version.sql
│   │   ├── 002_trg_facturas_row_version.sql
│   │   ├── 003_trg_auditoria_reservas.sql
│   │   ├── 004_trg_auditoria_pagos.sql
│   │   └── 005_trg_auditoria_facturas.sql
│   │
│   ├── Seeds
│   │   ├── 001_seed_roles.sql
│   │   ├── 002_seed_admin_user.sql
│   │   ├── 003_seed_catalogos_base.sql
│   │   └── 004_seed_atracciones_demo.sql
│   │
│   └── Views
│       ├── 001_vw_atracciones_disponibles.sql
│       └── 002_vw_reservas_detalle.sql
│
├── Migrations
│   └── migraciones EF Core
│
└── AtraccionesTuristicas.Backend.LA.DataAccess.csproj

## Reglas de base de datos

1. No usar esquema dbo.
2. No usar esquema crm.
3. No usar modelBuilder.HasDefaultSchema("dbo").
4. No usar builder.ToTable("tabla", "dbo").
5. No usar builder.ToTable("tabla", "crm").
6. Usar nombres reales de tablas en PostgreSQL.
7. El DDL entregado es referencia del modelo, pero la creación oficial debe quedar registrada en migraciones EF Core.
8. No ejecutar el DDL completo manualmente como reemplazo de migraciones.
9. Las funciones, triggers, vistas y seeds deben quedar dentro de Scripts y aplicarse mediante migraciones con migrationBuilder.Sql(...).
10. Las migraciones deben poder ejecutarse contra PostgreSQL desplegado en Azure.
11. Las migraciones no deben depender de rutas absolutas locales.
12. No ejecutar migraciones automáticamente al iniciar la API en Render. Para despliegue, las migraciones se deben aplicar manualmente o mediante un proceso controlado.

## Regla sobre contratos API

El modelo debe respetar los contratos API definidos para integración.

Principio general:

- Los ID internos numéricos se usan para relaciones internas de base de datos.
- Los GUID se usan como identificadores externos en contratos API.
- La API no debe exponer como identificador principal los IDs internos cuando exista GUID.
- Las entidades principales deben tener columna GUID única.

Ejemplo:

- at_id: identificador interno.
- at_guid: identificador externo para API.
- hor_id: identificador interno.
- hor_guid: identificador externo para API.
- tck_id: identificador interno.
- tck_guid: identificador externo para API.
- rev_id: identificador interno.
- rev_guid: identificador externo para API.

Los query repositories deben estar preparados para consultar por GUID cuando el contrato API lo requiera.

## Ajustes obligatorios antes de la migración inicial

Antes de crear InitialSchema, ajustar el modelo para alinear la base de datos con el contrato API:

1. Quitar toda referencia a dbo.
2. Horario debe depender de atraccion, no de ticket.
   - horario debe tener at_id.
   - ticket también depende de atraccion.
   - reserva referencia horario.
   - reserva_detalle referencia ticket.
3. La disponibilidad real se controla en horario.hor_cupos_disponibles.
4. No usar ticket.tck_cupos_disponibles como fuente principal de disponibilidad.
5. Reserva debe tener estados claros:
   - PENDIENTE
   - PAGADA
   - CONFIRMADA
   - CANCELADA
   - EXPIRADA
   - USADA
   - NO_SHOW
6. Agregar rev_fecha_expiracion_utc para permitir expiración de reservas pendientes.
7. Agregar o confirmar tabla pagos, porque existe PagoEntity, PagoRepository y función fn_confirmar_pago.
8. Agregar o confirmar tabla reserva_estado_historial, porque existe ReservaEstadoHistorialEntity y se requiere trazabilidad de cambios de estado.
9. Agregar id_codigo en idioma para soportar filtros como es, en, fr, it, de, ru, pt, ja, ar, pl.
10. Agregar cat_tagname en categoria para soportar filtros tipo y subtipo.
11. Agregar soporte de imagen principal:
    - ima_es_principal
    - ima_orden
12. Definir soporte para etiquetas:
    - free_cancellation
    - skip_the_line
13. Definir moneda:
    - ticket.tck_moneda default USD
    - reservas.rev_moneda default USD
    - facturas.fac_moneda default USD, si aplica.
14. Definir cómo representar incluye y no_incluye:
    - opción recomendada: inc_tipo con valores INCLUYE, NO_INCLUYE, ETIQUETA.
15. Agregar campos de auditoría técnica cuando el modelo lo requiera:
    - creado_en_utc o equivalente.
    - actualizado_en_utc o equivalente.
    - eliminado_en_utc o equivalente.
    - creado_por o equivalente.
    - actualizado_por o equivalente.
16. Mantener soft delete en tablas de catálogo y operación cuando aplique.
17. No aplicar soft delete a tablas puramente históricas o de auditoría si el modelo no lo requiere.

## Relación esperada para reservas

La relación correcta para cumplir el contrato API es:

atraccion
   ├── ticket
   └── horario

reservas
   ├── hor_id
   └── reserva_detalle
          └── tck_id

No usar:

atraccion
   └── ticket
        └── horario

La API POST /reservas recibe un hor_guid y una lista de tck_guid con cantidades, por eso el horario representa el slot de la atracción y los tickets representan los tipos de participantes/precios.

## Tablas esperadas

### Identidad y acceso

- roles
- usuario
- usuarioxroles

### Cliente

- clientes
- datos_facturacion

### Catálogo

- destino
- categoria
- idioma
- incluye
- imagen
- atraccion

### Relaciones de catálogo

- categoria_atraccion
- idioma_atraccion
- imagen_atraccion
- atraccion_incluye

### Operación

- ticket
- horario
- reservas
- reserva_detalle
- reserva_estado_historial
- pagos
- facturas
- resenia

### Auditoría

- auditoria_log

## Entidades V1 y V2

Aunque la V1 mínima viable no exponga todos los módulos por API, DataAccess puede tener el modelo completo si el DDL final ya lo contempla.

V1 funcional mínima:

- roles
- usuario
- usuarioxroles
- clientes
- destino
- categoria
- incluye
- imagen
- atraccion
- atraccion_incluye
- imagen_atraccion
- ticket
- horario
- reservas
- reserva_detalle
- reserva_estado_historial
- pagos
- datos_facturacion
- facturas
- auditoria_log

Soporte que puede quedar listo para V2:

- idioma
- idioma_atraccion
- resenia

No eliminar estas tablas si ya forman parte del modelo final y no rompen la V1.

## Responsabilidad de DataAccess

La capa DataAccess sí debe:

- Mapear entidades a tablas.
- Configurar PK, FK, índices, constraints, defaults y relaciones.
- Exponer DbSet en AtraccionesDbContext.
- Crear repositorios CRUD.
- Crear query repositories para consultas complejas.
- Mantener scripts SQL ordenados.
- Soportar migraciones.
- Usar async/await.
- Usar AsNoTracking en consultas de lectura.
- Usar tracking solo cuando se requiere actualizar.
- Mantener auditoría y soft delete cuando el modelo lo defina.
- Dejar preparadas consultas por GUID para contratos API.
- Dejar preparadas funciones de base de datos para procesos críticos.

La capa DataAccess no debe:

- Crear controllers.
- Crear DTOs de API.
- Crear DTOs de Business.
- Crear responses HTTP.
- Emitir JWT.
- Manejar CORS.
- Leer .env directamente.
- Leer configuración de Render.
- Hacer autorización por rol.
- Aplicar reglas de negocio complejas.
- Decidir si un usuario puede o no ejecutar una acción.
- Mezclar lógica de Business o API.

## Orden obligatorio de implementación

Implementar por fases. No avanzar a la siguiente fase si la anterior no compila.

### Fase 1: Preparación y Common

Crear o validar:

Common/PagedResult.cs
Common/SortDirection.cs
Common/DatabaseConstants.cs

DatabaseConstants debe centralizar:

- Nombres de tablas.
- Estados de registros.
- Estados de reservas.
- Estados de pagos.
- Moneda por defecto.
- Valores comunes.

No incluir Schema = "dbo".
No incluir Schema = "crm".

Valores mínimos recomendados:

- EstadoActivo = "A"
- EstadoInactivo = "I"
- ReservaPendiente = "PENDIENTE"
- ReservaPagada = "PAGADA"
- ReservaConfirmada = "CONFIRMADA"
- ReservaCancelada = "CANCELADA"
- ReservaExpirada = "EXPIRADA"
- ReservaUsada = "USADA"
- ReservaNoShow = "NO_SHOW"
- PagoPendiente = "PENDIENTE"
- PagoAprobado = "APROBADO"
- PagoRechazado = "RECHAZADO"
- MonedaDefault = "USD"

### Fase 2: Scripts base

Crear:

Scripts/Extensions/001_create_pgcrypto.sql

Contenido:

CREATE EXTENSION IF NOT EXISTS pgcrypto;

Este script debe aplicarse en la migración inicial antes de crear tablas que usen gen_random_uuid().

Si los scripts SQL se van a leer desde migraciones usando File.ReadAllText, configurar el .csproj para copiarlos al directorio de salida o usar una utilidad interna de lectura de scripts.

Ejemplo de configuración permitida en DataAccess.csproj:

<ItemGroup>
  <None Include="Scripts\**\*.sql" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>

Si no se configura copia de scripts, entonces el SQL debe insertarse directamente dentro de la migración con migrationBuilder.Sql(...).

### Fase 3: Entities

Crear entidades en este orden:

1. Identity
   - RolEntity
   - UsuarioEntity
   - UsuarioRolEntity

2. Cliente
   - ClienteEntity
   - DatosFacturacionEntity

3. Catalogo
   - DestinoEntity
   - CategoriaEntity
   - IdiomaEntity
   - IncluyeEntity
   - ImagenEntity
   - AtraccionEntity

4. CatalogoRelaciones
   - CategoriaAtraccionEntity
   - IdiomaAtraccionEntity
   - ImagenAtraccionEntity
   - AtraccionIncluyeEntity

5. Operacion
   - TicketEntity
   - HorarioEntity
   - ReservaEntity
   - ReservaDetalleEntity
   - ReservaEstadoHistorialEntity
   - PagoEntity
   - FacturaEntity
   - ReseniaEntity

6. Auditoria
   - AuditoriaLogEntity

Cada entidad debe respetar los nombres físicos de columnas del modelo PostgreSQL.

Convención recomendada:

- Usar propiedades cercanas a los nombres físicos: at_id, at_guid, rev_id, rev_guid, etc.
- Usar Guid para columnas UUID.
- Usar int para IDs identity.
- Usar long para row_version si existe.
- Usar decimal para valores monetarios.
- Usar DateTime para timestamp/timestamptz.
- Usar string para estados y códigos.
- Usar bool para columnas booleanas reales.
- Usar nullable cuando la columna permita NULL.

### Fase 4: Configurations

Crear configuraciones en el mismo orden que las entidades.

Cada configuración debe incluir:

- ToTable("nombre_tabla") sin esquema.
- Primary key.
- Unique indexes para GUID.
- Foreign keys.
- Longitudes.
- Precision para numeric.
- Defaults.
- Checks relevantes mediante HasCheckConstraint cuando aplique.
- Relaciones de navegación cuando aplique.
- Índices para columnas de búsqueda frecuente.

No configurar respuestas HTTP ni lógica de negocio aquí.

Defaults recomendados:

- UUID: gen_random_uuid()
- Fecha creación: CURRENT_TIMESTAMP
- Estado: 'A'
- Moneda: 'USD'
- Row version: 1

Checks recomendados:

- Estados permitidos.
- Montos no negativos.
- Cupos no negativos.
- Cantidades mayores a cero.
- Fechas de expiración coherentes.
- Monedas con longitud fija si aplica.
- Email con validación básica si el modelo lo contempla.

### Fase 5: AtraccionesDbContext

Crear:

Context/AtraccionesDbContext.cs

Debe incluir DbSet para todas las entidades.

Debe aplicar todas las configuraciones preferiblemente con:

modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtraccionesDbContext).Assembly);

También es aceptable usar ApplyConfiguration manual si se desea mayor control.

No usar:

modelBuilder.HasDefaultSchema("dbo");

No usar:

modelBuilder.HasDefaultSchema("crm");

No configurar cadena de conexión dentro del DbContext.

### Fase 6: Migración inicial

Crear la migración:

InitialSchema

Antes de aplicarla, revisar que:

- No aparezca EnsureSchema("dbo").
- No aparezca EnsureSchema("crm").
- No aparezca schema: "dbo".
- No aparezca schema: "crm".
- No aparezca ToTable con esquema.
- Se aplique CREATE EXTENSION IF NOT EXISTS pgcrypto.
- Las FK estén en orden correcto.
- horario tenga at_id y no dependa de ticket.
- ticket tenga at_id.
- reservas tenga hor_id.
- reserva_detalle tenga rev_id y tck_id.
- reservas tenga rev_fecha_expiracion_utc.
- reservas tenga estado expresivo.
- pagos exista si PagoEntity existe.
- reserva_estado_historial exista si ReservaEstadoHistorialEntity existe.
- Las columnas GUID tengan índices únicos.
- Los nombres de tablas coincidan con el modelo PostgreSQL.

Comando esperado:

dotnet ef migrations add InitialSchema --project .\AtraccionesTuristicas.Backend.LA.DataAccess --startup-project .\AtraccionesTuristicas.Backend.LA.Api --context AtraccionesDbContext

Aplicación controlada:

dotnet ef database update --project .\AtraccionesTuristicas.Backend.LA.DataAccess --startup-project .\AtraccionesTuristicas.Backend.LA.Api --context AtraccionesDbContext

### Fase 7: Scripts de funciones

Crear scripts:

Scripts/Functions/001_fn_increment_row_version_generic.sql
Scripts/Functions/002_fn_crear_reserva.sql
Scripts/Functions/003_fn_cancelar_reserva.sql
Scripts/Functions/004_fn_expirar_reservas_pendientes.sql
Scripts/Functions/005_fn_confirmar_pago.sql
Scripts/Functions/006_fn_generar_factura.sql
Scripts/Functions/007_fn_registrar_auditoria.sql

No usar prefijo dbo.

Ejemplo correcto:

CREATE OR REPLACE FUNCTION fn_crear_reserva(...)

Ejemplo incorrecto:

CREATE OR REPLACE FUNCTION dbo.fn_crear_reserva(...)

Las funciones deben aplicarse con una migración separada:

AddDatabaseFunctions

Responsabilidad esperada de funciones:

- fn_crear_reserva:
  - Validar horario activo.
  - Validar cupos disponibles.
  - Crear reserva pendiente.
  - Crear reserva_detalle.
  - Descontar cupos de horario.
  - Registrar fecha de expiración.
  - Registrar historial de estado.

- fn_cancelar_reserva:
  - Validar reserva cancelable.
  - Cambiar estado a CANCELADA.
  - Devolver cupos si corresponde.
  - Registrar historial de estado.

- fn_expirar_reservas_pendientes:
  - Buscar reservas PENDIENTE vencidas.
  - Cambiar estado a EXPIRADA.
  - Devolver cupos.
  - Registrar historial.

- fn_confirmar_pago:
  - Validar reserva PENDIENTE.
  - Validar que no esté expirada.
  - Registrar pago.
  - Cambiar reserva a PAGADA o CONFIRMADA según la regla definida.
  - Invocar o dejar lista la generación de factura.

- fn_generar_factura:
  - Generar factura para pago aprobado.
  - Evitar duplicidad de factura.
  - Calcular subtotal, impuestos y total.

- fn_registrar_auditoria:
  - Registrar acción, tabla, registro, usuario, fecha, datos anteriores/nuevos e IP si aplica.

### Fase 8: Scripts de triggers

Crear scripts:

Scripts/Triggers/001_trg_clientes_row_version.sql
Scripts/Triggers/002_trg_facturas_row_version.sql
Scripts/Triggers/003_trg_auditoria_reservas.sql
Scripts/Triggers/004_trg_auditoria_pagos.sql
Scripts/Triggers/005_trg_auditoria_facturas.sql

Aplicarlos con la migración:

AddDatabaseTriggers

No usar dbo.
No usar crm.

### Fase 9: Scripts de views

Crear scripts:

Scripts/Views/001_vw_atracciones_disponibles.sql
Scripts/Views/002_vw_reservas_detalle.sql

Aplicarlos con la migración:

AddDatabaseViews

Las vistas deben facilitar consultas públicas y administrativas, pero no deben reemplazar las tablas ni la lógica de negocio.

### Fase 10: Seeds

Crear scripts:

Scripts/Seeds/001_seed_roles.sql
Scripts/Seeds/002_seed_admin_user.sql
Scripts/Seeds/003_seed_catalogos_base.sql
Scripts/Seeds/004_seed_atracciones_demo.sql

Aplicarlos con la migración:

SeedInitialData

Los seeds deben ser idempotentes.

Usar:

- INSERT ... WHERE NOT EXISTS
- ON CONFLICT DO NOTHING
- ON CONFLICT (...) DO UPDATE cuando corresponda

Seeds mínimos:

- Rol CLIENTE.
- Rol ADMIN.
- Usuario administrador inicial.
- Relación usuario administrador con rol ADMIN.
- Categorías base.
- Destinos base.
- Idiomas base si idioma forma parte del modelo.
- Incluye base si se requiere para atracciones demo.

No guardar contraseñas en texto plano en seeds finales.
Si se usa contraseña temporal para desarrollo, dejar comentario TODO indicando que debe reemplazarse por hash seguro.

### Fase 11: Repositories

Primero crear interfaces, luego implementaciones.

Crear:

Repositories/Interfaces/IRepositoryBase.cs
Repositories/RepositoryBase.cs

Luego crear interfaces e implementaciones para:

- Rol
- Usuario
- UsuarioRol
- Cliente
- DatosFacturacion
- Destino
- Categoria
- Idioma
- Incluye
- Imagen
- Atraccion
- CategoriaAtraccion
- IdiomaAtraccion
- ImagenAtraccion
- AtraccionIncluye
- Ticket
- Horario
- Reserva
- ReservaDetalle
- ReservaEstadoHistorial
- Pago
- Factura
- Resenia
- AuditoriaLog

Los repositorios CRUD no deben construir responses HTTP ni DTOs.

Métodos mínimos en RepositoryBase:

- ObtenerPorIdAsync
- ListarAsync
- AgregarAsync
- Actualizar
- Remover

Métodos recomendados en repositorios específicos:

UsuarioRepository:
- ObtenerPorLoginAsync
- ObtenerPorCorreoAsync
- ObtenerConRolesAsync

ClienteRepository:
- ObtenerPorGuidAsync
- ObtenerPorIdentificacionAsync
- ObtenerPorUsuarioIdAsync
- ObtenerParaActualizarAsync

AtraccionRepository:
- ObtenerPorGuidAsync
- ObtenerParaActualizarAsync
- ListarActivasAsync

TicketRepository:
- ObtenerPorGuidAsync
- ListarActivosPorAtraccionAsync

HorarioRepository:
- ObtenerPorGuidAsync
- ObtenerParaActualizarAsync
- ListarDisponiblesPorAtraccionAsync

ReservaRepository:
- ObtenerPorGuidAsync
- ObtenerPorCodigoAsync
- ObtenerParaActualizarAsync
- ObtenerConDetalleAsync

PagoRepository:
- ObtenerPorGuidAsync
- ObtenerPorReservaIdAsync

FacturaRepository:
- ObtenerPorGuidAsync
- ObtenerPorReservaIdAsync
- ObtenerPorPagoIdAsync

AuditoriaLogRepository:
- AgregarAsync
- ConsultarPorTablaAsync si aplica

Las funciones de base de datos deben invocarse desde los repositorios existentes de operación:

- ReservaRepository para fn_crear_reserva, fn_cancelar_reserva, fn_expirar_reservas_pendientes.
- PagoRepository para fn_confirmar_pago.
- FacturaRepository para fn_generar_factura.
- AuditoriaLogRepository o repositorios operativos para fn_registrar_auditoria.

No crear un nuevo StoredProcedureRepository si no está en la estructura aprobada.

### Fase 12: Queries

Crear interfaces e implementaciones:

Queries/Interfaces/IClienteQueryRepository.cs
Queries/Interfaces/IAtraccionQueryRepository.cs
Queries/Interfaces/IHorarioQueryRepository.cs
Queries/Interfaces/IReservaQueryRepository.cs
Queries/Interfaces/IPagoQueryRepository.cs
Queries/Interfaces/IFacturaQueryRepository.cs
Queries/Interfaces/IAuditoriaLogQueryRepository.cs

Implementar:

Queries/ClienteQueryRepository.cs
Queries/AtraccionQueryRepository.cs
Queries/HorarioQueryRepository.cs
Queries/ReservaQueryRepository.cs
Queries/PagoQueryRepository.cs
Queries/FacturaQueryRepository.cs
Queries/AuditoriaLogQueryRepository.cs

Todos los QueryRepository deben usar AsNoTracking().

AtraccionQueryRepository debe quedar preparado para:

- Listar atracciones públicas.
- Filtrar por ciudad.
- Filtrar por tipo.
- Filtrar por subtipo.
- Filtrar por etiqueta.
- Filtrar por idioma.
- Filtrar por calificación mínima.
- Filtrar por horario.
- Filtrar por disponibilidad.
- Ordenar por trending.
- Ordenar por lowest_price.
- Ordenar por highest_weighted_rating.
- Paginar con page y limit.

HorarioQueryRepository debe quedar preparado para:

- Listar horarios disponibles por atracción.
- Filtrar por fecha.
- Filtrar solo horarios activos.
- Filtrar solo horarios con cupos disponibles.

ReservaQueryRepository debe quedar preparado para:

- Consultar reserva por GUID.
- Consultar reserva por código.
- Consultar reservas por cliente.
- Consultar reservas por estado.
- Consultar reservas por rango de fecha.
- Incluir detalle, horario, atracción y líneas de ticket cuando aplique.

PagoQueryRepository debe quedar preparado para:

- Consultar pagos por reserva.
- Consultar pagos por cliente.
- Consultar pagos por estado.
- Consultar pagos por fecha.

FacturaQueryRepository debe quedar preparado para:

- Consultar facturas por cliente.
- Consultar facturas por número.
- Consultar facturas por reserva.
- Consultar facturas por fecha.

AuditoriaLogQueryRepository debe quedar preparado para:

- Consultar por tabla.
- Consultar por acción.
- Consultar por usuario.
- Consultar por fecha.
- Paginación.

### Fase 13: Validación final

Al terminar cada fase:

- Compilar.
- No dejar warnings graves.
- No romper la estructura.
- No crear carpetas nuevas sin autorización.
- No usar schema dbo.
- No usar SQL Server.
- No usar crm.
- No mezclar lógica de negocio.
- No crear controllers.
- No crear DTOs de Business o API.
- No leer variables de entorno desde DataAccess.
- No quemar cadenas de conexión.

Al terminar toda la capa:

- Ejecutar migraciones.
- Verificar tablas en PostgreSQL.
- Verificar funciones.
- Verificar triggers.
- Verificar vistas.
- Verificar seeds.
- Probar queries básicas.
- Confirmar que la base se crea en public.
- Confirmar que los GUID externos existen y son únicos.
- Confirmar que horario depende de atraccion.
- Confirmar que ticket depende de atraccion.
- Confirmar que reservas depende de horario.
- Confirmar que reserva_detalle depende de ticket.

## Criterios de aceptación de DataAccess

La capa se considera terminada cuando:

1. El proyecto AtraccionesTuristicas.Backend.LA.DataAccess compila.
2. No existe ninguna referencia a SQL Server.
3. No existe ninguna referencia a dbo.
4. No existe ninguna referencia a crm.
5. AtraccionesDbContext expone todos los DbSet.
6. Todas las entidades tienen su configuración.
7. Las migraciones se pueden crear desde el proyecto DataAccess usando Api como startup-project.
8. InitialSchema genera tablas en PostgreSQL public.
9. Los scripts SQL existen y están organizados.
10. Los repositorios existen y no contienen lógica HTTP.
11. Los query repositories existen y usan AsNoTracking.
12. El modelo respeta los contratos API de reserva:
    - hor_guid para horario.
    - tck_guid para tickets.
    - reserva con detalle de tickets.
    - disponibilidad en horario.
13. El proyecto queda listo para implementar DataManagement.