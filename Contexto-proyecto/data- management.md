La presente guía original es conceptual y usa Clientes + SQL Server como ejemplo. Para este proyecto, el agente debe implementar AtraccionesTuristicas.Backend.LA.DataManagement con PostgreSQL, Npgsql, migraciones EF Core, modelo sin esquema y la estructura oficial indicada.

Capa 2: Gestión de Datos.
Aquí ya no hablamos directamente con SQL Server.
Ahora nos ubicamos encima de Acceso a Datos y construimos una capa que:
• coordina repositorios
• controla transacciones
• centraliza operaciones de persistencia
• prepara la información para la lógica de negocio
• desacopla la capa de negocio de EF Core y del detalle técnico de base de datos
Esta es una capa muy importante porque aquí empieza a notarse una visión más madura
de arquitectura de software.
1. Qué es la capa de Gestión de Datos
La capa de Gestión de Datos es una capa intermedia entre:
• Acceso a Datos
• Lógica de Negocio
Su propósito es orquestar la persistencia.
No se conecta directamente a HTTP.
No expone endpoints.
No aplica reglas funcionales profundas del negocio.
Tampoco debería contener SQL ni DbContext.
Su rol es coordinar técnicamente el acceso a datos para que la capa de negocio trabaje
con servicios de datos limpios y consistentes.
2. Ubicación en la arquitectura general
Cliente HTTP
↓
API REST
↓
Lógica de Negocio
↓
Gestión de Datos
↓
Acceso a Datos
↓
SQL Server
3. Rol arquitectónico de esta capa
Esta capa resuelve problemas muy comunes en sistemas empresariales:
• evitar que la lógica de negocio hable con muchos repositorios a la vez
• centralizar el guardado transaccional
• agrupar operaciones relacionadas
• convertir entidades persistentes en modelos más cómodos para las capas superiores
• ocultar EF Core y la infraestructura a la lógica del dominio
En otras palabras:
la capa de negocio no debería preocuparse por cómo se persiste algo, sino por qué se
necesita hacer.
4. Diagrama arquitectónico de la capa 2
5. Idea central de esta capa
La capa 2 se construye alrededor de dos ideas arquitectónicas:
A. Servicios de datos
Son clases que agrupan operaciones de persistencia útiles para la capa de negocio.
```
Ejemplo:
```
• buscar cliente por identificación
• registrar cliente
• actualizar cliente
• listar clientes paginados
B. Unit of Work
Es el componente que centraliza:
• repositorios
• SaveChanges
• transacciones
• consistencia de persistencia
6. Componentes que vamos a usar
La propuesta de esta capa sería:
Microservicio.Clientes.DataManagement
│
├── Interfaces
│ ├── IUnitOfWork.cs
│ ├── IClienteDataService.cs
│ ├── IUsuarioDataService.cs
│ └── IAuditoriaDataService.cs
│
├── Services
│ ├── UnitOfWork.cs
│ ├── ClienteDataService.cs
│ ├── UsuarioDataService.cs
│ └── AuditoriaDataService.cs
│
├── Models
│ ├── ClienteDataModel.cs
│ ├── ClienteFiltroDataModel.cs
│ ├── UsuarioDataModel.cs
│ ├── LoginDataModel.cs
│ └── AuditoriaDataModel.cs
│
├── Mappers
│ ├── ClienteDataMapper.cs
│ ├── UsuarioDataMapper.cs
│ └── AuditoriaDataMapper.cs
│
└── Common
└── DataPagedResult.cs
7. Descripción de cada componente
7.1 IUnitOfWork
Qué es
Es la interfaz que define el contrato de la unidad de trabajo.
Qué hace
Agrupa los repositorios y ofrece un punto único para:
• confirmar cambios
• manejar transacciones
• exponer dependencias de persistencia
Por qué es importante
Sin UnitOfWork, la capa de negocio o los servicios de datos podrían terminar usando
varios repositorios de forma desordenada, haciendo múltiples guardados sin control.
Ejemplo conceptual
ClienteDataService
├── usa ClienteRepository
├── usa AuditoriaRepository
└── guarda una sola vez con UnitOfWork
7.2 UnitOfWork
Qué es
Es la implementación concreta de IUnitOfWork.
Qué hace
• contiene referencias a repositorios
• usa el DbContext indirectamente a través de DataAccess
```
• ejecuta SaveChangesAsync()
```
• puede abrir y cerrar transacciones
Por qué es importante
Permite que varias operaciones formen parte de una misma unidad consistente de
persistencia.
7.3 IClienteDataService
Qué es
Es la interfaz del servicio de datos para clientes.
Qué hace
Define operaciones orientadas a la persistencia útil para la capa de negocio, por
```
ejemplo:
```
• obtener cliente por id
• obtener por identificación
• listar paginado
• crear registro
• actualizar registro
• eliminar lógicamente
Diferencia frente a IClienteRepository
El repositorio trabaja más cerca de la tabla.
El servicio de datos trabaja a un nivel más alto, coordinando varios elementos de
persistencia si hace falta.
7.4 ClienteDataService
Qué es
Es la implementación del servicio de datos de clientes.
Qué hace
• usa IUnitOfWork
• consume IClienteRepository
• consume consultas especializadas
• convierte entidades a modelos de datos
• centraliza operaciones de persistencia de clientes
Ejemplo
Si al crear un cliente quieres:
• guardar el cliente
• registrar auditoría
• confirmar cambios una sola vez
eso debe hacerse aquí.
7.5 IUsuarioDataService
Qué es
Contrato del servicio de datos para usuarios de aplicación.
Qué hace
Permite consultar usuarios de aplicación para autenticación y autorización futura.
Casos típicos
• obtener usuario por username
• listar roles del usuario
• validar que está activo
7.6 UsuarioDataService
Qué es
Implementación del servicio de datos para usuarios.
Qué hace
• consulta usuarios
• consulta roles
• prepara datos para autenticación JWT en capas superiores
7.7 IAuditoriaDataService
Qué es
Contrato del servicio de datos para auditoría.
Qué hace
• registrar eventos de auditoría
• consultar trazas si fuese necesario
7.8 AuditoriaDataService
Qué es
Implementación del servicio de auditoría.
Qué hace
• recibe modelos de auditoría
• los convierte en entidades
• los persiste usando UnitOfWork
7.9 Models
Aquí no usamos directamente las entidades persistentes de DataAccess en todas partes.
Para mantener el desacoplamiento, definimos modelos de datos.
Ejemplo
• ClienteEntity pertenece a persistencia
• ClienteDataModel pertenece a gestión de datos
Esto evita que las capas superiores dependan del modelo físico de la base.
7.10 Mappers
Qué son
Clases responsables de transformar:
• Entity ↔ DataModel
Por qué son importantes
Porque ayudan a desacoplar la capa de gestión de datos de la representación física
exacta de EF Core.
7.11 DataPagedResult
Qué es
Un modelo paginado propio de esta capa.
Por qué conviene
Porque así no arrastras directamente el PagedResult de DataAccess hacia negocio.
Cada capa puede exponer su propio modelo.

8. Principios arquitectónicos que
justifican esta capa
8.1 Separación de responsabilidades
La lógica de negocio no debe saber:
• cómo funciona EF Core
• qué repositorio invocar primero
• cuándo hacer SaveChanges
Eso lo resuelve la capa de gestión de datos.
8.2 Desacoplamiento
La capa de negocio depende de interfaces como:
• IClienteDataService
• IUsuarioDataService
y no de:
• DbContext
• DbSet
• EF Core
• SQL Server
8.3 Reutilización
Varias reglas de negocio pueden necesitar la misma operación de persistencia.
En lugar de repetir lógica, se encapsula aquí.
8.4 Escalabilidad
Hoy el microservicio es pequeño.
Mañana podrías tener:
• clientes
• direcciones
• teléfonos
• sesiones
• refresh tokens
• historial
Esta capa ayuda a crecer de manera ordenada.
8.5 Preparación para transacciones
Si una operación requiere:
• insertar cliente
• insertar auditoría
• actualizar otra tabla
todo debe formar parte de una misma unidad lógica de persistencia.
9. Relación con la seguridad futura
Aunque JWT, autenticación y autorización se implementarán en la API y en negocio,
esta capa debe dejar listo el soporte de datos para eso.
Por ejemplo:
• consultar usuario por nombre
• obtener roles de usuario
• verificar si está activo
• registrar auditoría de login
Entonces esta capa ya debe pensarse como soporte directo para:
• autenticación
• autorización
• trazabilidad
10. Qué no debe hacer esta capa
Esta capa no debe:
• emitir JWT
• validar credenciales directamente contra HTTP
• manejar controllers
• devolver respuestas HTTP
• hacer validaciones funcionales profundas del negocio
• conocer CORS o versionamiento
Eso corresponde a capas superiores.
11. Qué sí debe hacer
Sí debe:
• coordinar repositorios
• manejar guardados
• mapear entre entidades y modelos
• preparar paginación
• encapsular acceso técnico a los datos
• soportar trazabilidad y usuarios
12. Diagrama de componentes de la capa
2
13. Flujo de ejemplo: crear cliente
Lógica de Negocio
↓
IClienteDataService
↓
ClienteDataService
↓
IUnitOfWork
```
├── ClienteRepository.AgregarAsync()
```
```
├── AuditoriaRepository.AgregarAsync()
```
```
└── SaveChangesAsync()
```
↓
DataAccess
↓
SQL Server
14. Propuesta para arrancar
Para no hacer demasiado grande esta segunda capa desde el primer momento,
empezamos con estos componentes mínimos:
Interfaces
• IUnitOfWork
• IClienteDataService
Implementaciones
• UnitOfWork
• ClienteDataService
Modelos
• ClienteDataModel
• ClienteFiltroDataModel
Mapper
• ClienteDataMapper
Luego, en una segunda iteración de esta misma capa:
• IUsuarioDataService
• UsuarioDataService
• IAuditoriaDataService
• AuditoriaDataService
15. Resultado esperado de esta capa
Cuando terminemos esta capa, deberíamos tener:
• una unidad de trabajo central
• un servicio de datos de clientes
• modelos de datos desacoplados de EF
• mapeadores entre entidades y modelos
• persistencia más limpia para negocio
• base lista para construir la capa 3

PASO 11. Crear ClienteDataModel
Ubicación
Models/ClienteDataModel.cs
Qué es
Modelo desacoplado de EF Core que usará la capa de gestión de datos.

PASO 12. Crear ClienteFiltroDataModel
Ubicación
Models/ClienteFiltroDataModel.cs
Qué es
Modelo para filtros y búsquedas paginadas.

PASO 13. Crear DataPagedResult<T>
Ubicación
Models/DataPagedResult.cs
Qué es
Modelo paginado propio de la capa 2.

PARTE 4. MAPEADOR
PASO 14. Crear ClienteDataMapper
Ubicación
Mappers/ClienteDataMapper.cs
Qué hace
Convierte entre:
• ClienteEntity
• ClienteDataModel

PARTE 5. UNIT OF WORK
PASO 15. Crear IUnitOfWork
Ubicación
Interfaces/IUnitOfWork.cs
Qué hace
Exponer repositorios y el guardado central.

PASO 16. Crear UnitOfWork
Ubicación
Services/UnitOfWork.cs
Qué hace
Centraliza repositorios y SaveChangesAsync.

PARTE 6. SERVICIO DE DATOS DE
CLIENTE
PASO 17. Crear IClienteDataService
Ubicación
Interfaces/IClienteDataService.cs
Qué hace
Define el contrato del servicio de gestión de datos para cliente.

PASO 18. Crear ClienteDataService
Ubicación
Services/ClienteDataService.cs
Qué hace
• usa repositorio de cliente
• usa consulta paginada
• mapea entidades
• guarda cambios
• hace borrado lógico