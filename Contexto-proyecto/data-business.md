CAPA 3: LÓGICA DE NEGOCIO
```
(Business Layer)
```
🎯 Objetivo
La capa de lógica de negocio es la que:
• interpreta los requerimientos del negocio
• aplica reglas y validaciones
```
• orquesta los datos (NO los obtiene directamente)
```
• transforma datos en respuestas funcionales
• controla el flujo de los casos de uso
Es el cerebro del sistema
Ubicación en la arquitectura
```
API REST (Controllers)
```
↓
```
Lógica de Negocio (Business)
```
↓
```
Gestión de Datos (DataManagement)
```
↓
```
Acceso a Datos (DataAccess)
```
↓
SQL Server
RESPONSABILIDADES CLAVE
✔️Esta capa SÍ hace
• Validaciones de negocio
```
• Reglas (ej: cliente no puede duplicar identificación)
```
• Orquestación de servicios
• Manejo de excepciones de negocio
```
• Transformación de modelos (Data → DTO)
```
```
• Seguridad funcional (roles, permisos)
```
• Preparación de respuestas para la API
❌ Esta capa NO hace
• NO accede directamente a EF Core
• NO usa DbContext
• NO escribe SQL
• NO conoce detalles de infraestructura
• NO devuelve entidades
COMPONENTES DE LA CAPA 3
La vamos a estructurar así:
Microservicio.Clientes.Business
│
├── Interfaces
│ ├── IClienteService.cs
│ └── IAuthService.cs
│
├── Services
│ ├── ClienteService.cs
│ └── AuthService.cs
│
├── DTOs
│ ├── Cliente
│ │ ├── CrearClienteRequest.cs
│ │ ├── ActualizarClienteRequest.cs
│ │ └── ClienteResponse.cs
│ │
│ └── Auth
│ ├── LoginRequest.cs
│ └── LoginResponse.cs
│
├── Validators
│ └── ClienteValidator.cs
│
├── Exceptions
│ ├── BusinessException.cs
│ ├── NotFoundException.cs
│ └── ValidationException.cs
│
└── Mappers
└── ClienteBusinessMapper.cs

EXPLICACIÓN DE CADA
COMPONENTE
1. Interfaces
IClienteService
Define qué puede hacer el sistema con clientes:
• crear
• actualizar
• eliminar
• buscar
Es el contrato que usa la API
IAuthService
```
Define:
```
• login
• validación de usuario
```
• generación de JWT (en API pero orquestado aquí)
```
2. Services
ClienteService
Es el corazón del negocio de clientes
```
Responsabilidades:
```
• validar datos
• evitar duplicados
• aplicar reglas
• llamar a DataManagement
• transformar datos
AuthService
Maneja autenticación
```
Responsabilidades:
```
• validar usuario y contraseña
• verificar estado del usuario
• obtener roles
• preparar claims para JWT
3. DTOs (Data Transfer Objects)
Son los modelos que usa la API
```
Ejemplos:
```
Request
• CrearClienteRequest
• LoginRequest
Response
• ClienteResponse
• LoginResponse
Separan la API del modelo interno
4. Validators
Aquí viven las reglas de validación
```
Ejemplo:
```
• identificación obligatoria
• email válido
• tipo cliente válido
NO usar DataAnnotations aquí → lógica real
5. Exceptions
Fundamental para arquitectura limpia
```
Tipos:
```
• BusinessException
• ValidationException
• NotFoundException
Permiten manejar errores correctamente en la API
6. Mappers
```
Transforman:
```
DTO ⇄ DataModel
```
Ejemplo:
```
CrearClienteRequest → ClienteDataModel
ClienteDataModel → ClienteResponse
FLUJO COMPLETO DE UNA
OPERACIÓN
Crear Cliente
Controller
↓
ClienteService
↓
Valida reglas
↓
```
Mapper (Request → DataModel)
```
↓
ClienteDataService
↓
UnitOfWork
↓
Repository
↓
DB
SEGURIDAD EN ESTA CAPA
Aquí se empieza a aplicar:
• validación de usuario activo
• validación de roles
• restricciones de negocio
```
Ejemplo:
```
• solo ADMIN puede eliminar
• vendedor solo puede consultar
PRINCIPIOS QUE ESTAMOS
APLICANDO
Esta capa implementa:
✔️ Clean Architecture
Separación clara de responsabilidades
✔️SOLID
• SRP → cada clase hace una sola cosa
• DIP → dependemos de interfaces
```
✔️Domain Driven Thinking (ligero)
```
Reglas del negocio aquí, no en DB ni API
“La capa 3 es donde el sistema deja de ser técnico y se vuelve inteligente.”
Codificación capa 3: Lógica de Negocio
La idea es construir una versión 1 funcional de la capa de negocio para que luego
podamos pasar a la API REST v1.
Objetivo de la capa 3
Construir el proyecto:
Microservicio.Clientes.Business
para que:
• reciba solicitudes de la API
• aplique reglas y validaciones de negocio
```
• use la capa 2 (DataManagement)
```
• prepare respuestas limpias para la API
• deje listo el login para JWT
Alcance de la versión 1
Vamos a implementar estos componentes:
Microservicio.Clientes.Business
│
├── Interfaces
│ ├── IClienteService.cs
│ └── IAuthService.cs
│
├── Services
│ ├── ClienteService.cs
│ └── AuthService.cs
│
├── DTOs
│ ├── Cliente
│ │ ├── CrearClienteRequest.cs
│ │ ├── ActualizarClienteRequest.cs
│ │ ├── ClienteResponse.cs
│ │ └── ClienteFiltroRequest.cs
│ │
│ └── Auth
│ ├── LoginRequest.cs
│ └── LoginResponse.cs
│
├── Validators
│ └── ClienteValidator.cs
│
├── Exceptions
│ ├── BusinessException.cs
│ ├── ValidationException.cs
│ ├── NotFoundException.cs
│ └── UnauthorizedBusinessException.cs
│
└── Mappers
└── ClienteBusinessMapper.cs
Qué hará esta capa
Cliente
• crear cliente
• actualizar cliente
• listar clientes
• buscar cliente por id
• buscar cliente por identificación
• eliminar lógicamente cliente
Auth
• validar username
• validar contraseña
• validar si el usuario está activo
• devolver información de login base para luego generar JWT en la API

PARTE 3. DTOs DE CLIENTE
Los DTOs son los modelos que la API usará para comunicarse con la capa de negocio.

PARTE 4. DTOs DE AUTENTICACIÓN

PARTE 5. VALIDADOR DE CLIENTE

PARTE 6. MAPPER DE NEGOCIO
Este mapper transforma DTOs ↔ DataModels.

PARTE 7. INTERFACES DE NEGOCIO

PARTE 8. SERVICIO DE NEGOCIO
DE CLIENTE

PARTE 9. SERVICIO DE
AUTENTICACIÓN V1
Aquí haremos una versión 1 simple y funcional. Como todavía no construimos
UsuarioDataService, nos apoyaremos en IUnitOfWork de capa 2.

PARTE 10. VALIDACIÓN FINAL DE
LA CAPA 3
Paso 20. Revisa las referencias
El proyecto Business debe referenciar:
• Microservicio.Clientes.DataManagement
Y como DataManagement ya referencia a DataAccess, no necesitas agregar referencia
directa a DataAccess en Business.