Implementa la capa AtraccionesTuristicas.Backend.LA.DataManagement siguiendo la arquitectura por capas del proyecto.

Contexto:
- DataAccess ya contiene entidades, configuraciones, AtraccionesDbContext, repositorios CRUD, query repositories, scripts SQL y migraciones.
- DataManagement debe referenciar únicamente DataAccess.
- No debe referenciar Api ni Business.
- No debe crear controllers, DTOs de API, respuestas HTTP, JWT, CORS ni Swagger.
- No debe leer appsettings.json, .env ni variables de entorno.
- No debe crear migraciones.

Objetivo:
Crear una capa de gestión de datos que coordine repositorios, centralice SaveChangesAsync, prepare transacciones, transforme Entity ↔ DataModel y exponga servicios de datos para Business.

Estructura creada:

Common/
- DataPagedResult.cs
- OperationResult.cs

Interfaces/
- IUnitOfWork.cs
- ITransactionManager.cs
- Subcarpetas Identity, Cliente, Catalogo, CatalogoRelaciones, Operacion, Auditoria con interfaces de servicios de datos.

Services/
- UnitOfWork.cs
- TransactionManager.cs
- Subcarpetas Identity, Cliente, Catalogo, CatalogoRelaciones, Operacion, Auditoria con implementaciones.

Models/
- Subcarpetas Identity, Cliente, Catalogo, CatalogoRelaciones, Operacion, Auditoria con DataModels.

Mappers/
- Subcarpetas Identity, Cliente, Catalogo, CatalogoRelaciones, Operacion, Auditoria con mappers Entity ↔ DataModel.

Reglas:
- UnitOfWork debe exponer todos los repositorios y query repositories definidos en DataAccess.
- DataServices deben usar IUnitOfWork.
- DataServices deben recibir y devolver DataModels, nunca Entities hacia capas superiores.
- Mappers deben convertir Entity ↔ DataModel.
- Query operations deben usar los QueryRepositories existentes de DataAccess.
- Crear métodos CRUD técnicos para catálogos.
- Crear métodos de consulta por GUID para entidades expuestas por API.
- Crear servicios operativos para reservas, pagos y facturas preparados para invocar funciones PostgreSQL desde repositorios.
- No aplicar reglas de negocio profundas.
- No crear lógica HTTP.
- No cambiar DataAccess salvo que sea necesario por errores de compilación.

Criterio de aceptación:
- El proyecto DataManagement compila.
- Business podrá consumir interfaces de DataManagement sin conocer DataAccess directamente.