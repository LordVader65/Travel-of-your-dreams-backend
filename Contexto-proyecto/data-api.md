La presente guía original es conceptual y usa Clientes + SQL Server como ejemplo. Para este proyecto, el agente debe implementar AtraccionesTuristicas.Backend.LA.Api con PostgreSQL, Npgsql, migraciones EF Core.

Capa 4: API REST v1
Esta capa es la que expone el microservicio al exterior. Aquí el sistema deja de ser solo
una arquitectura interna y se convierte en un servicio consumible por:
• frontend web
• app móvil
• Postman
• Swagger
• otros microservicios
Objetivo
La capa API REST tiene como propósito:
• exponer endpoints HTTP
• recibir requests y devolver responses
• aplicar autenticación y autorización
• usar HTTPS
• habilitar CORS
• aplicar versionamiento
• centralizar manejo de errores
• conectar la capa de negocio con el mundo exterior
En términos simples:
la capa 4 es la puerta de entrada del microservicio.
Ubicación en la arquitectura general
Cliente / Frontend / Postman / Otro servicio
↓
API REST
↓
Lógica de Negocio
↓
Gestión de Datos
↓
Acceso a Datos
↓
SQL Server / base de datos
Responsabilidad arquitectónica
Esta capa sí hace
• exponer rutas HTTP
• model binding
• serialización JSON
• autenticación JWT
• autorización por rol
• versionamiento de la API
• CORS
• HTTPS
• documentación con Swagger
• manejo global de excepciones
• inyección de dependencias
• transformar excepciones de negocio en respuestas HTTP
Esta capa no hace
• no implementa reglas de negocio profundas
• no consulta la base directamente
• no usa repositorios directamente
• no debe contener SQL
• no debe contener lógica funcional compleja
La lógica vive en la capa 3, no en los controllers.
Componentes de la capa 4

Diagrama de componentes de la API
REST
Estructura lógica de funcionamiento
HTTP Request
↓
Routing
↓
Middleware Pipeline
├── HTTPS
├── CORS
├── Authentication
├── Authorization
└── Exception Handling
↓
Controller
↓
Business Service
↓
Response HTTP JSON
Explicación de cada componente
1. Controllers
Los controladores son los puntos de entrada HTTP.

Responsabilidad
• recibir requests
• invocar servicios de negocio
• devolver respuestas HTTP
• no contener lógica compleja

2. Middleware
El middleware es clave en ASP.NET Core porque forma el pipeline de procesamiento
de cada request.
En V1 incluiremos:
• middleware global de excepciones
Responsabilidad
• capturar errores no controlados
• traducir excepciones de negocio a respuestas HTTP correctas
• evitar duplicar try/catch en cada controller
3. Authentication (JWT)
La API no debe confiar en cualquier solicitud.
Debe autenticar al usuario mediante JWT Bearer.
Qué hará
• validar token
• verificar firma
• verificar expiración
• leer claims
Claims típicos
• sub
• unique_name
• role
• email
4. Authorization
Después de autenticar, la API debe decidir qué puede hacer cada usuario. Trabajando con los roles definidos que usaran los usuarios.

5. HTTPS
Toda la API debe exponerse bajo HTTPS.
Por qué
Porque JWT, credenciales y datos sensibles no deben viajar en texto plano.
Rol arquitectónico
HTTPS no es opcional: es parte del diseño seguro del microservicio.
6. CORS
CORS controla qué orígenes pueden consumir la API desde navegador.
En V1
Permitiremos dominios específicos, por ejemplo:
• localhost Angular
• localhost React
• localhost Vue
Por qué
Porque el frontend puede estar en otro origen y, sin CORS, el navegador bloquearía la
solicitud.
7. Versionamiento
El versionamiento permite evolucionar la API sin romper clientes existentes.
Estrategia recomendada
Versionado por URL:
/api/v1/clientes
/api/v1/auth/login
Ventajas
• simple
• claro
• fácil de documentar
• muy didáctico para estudiantes
8. Swagger / OpenAPI
Swagger servirá para:
• documentar endpoints
• probar la API
• explicar la arquitectura
• probar JWT desde interfaz web
En V1
Lo configuraremos con soporte para bearer token.
9. Models de respuesta API
Conviene estandarizar cómo responde la API.
Modelos sugeridos
• ApiResponse<T>
• ApiErrorResponse

10. Settings
Necesitamos una clase para leer configuración de JWT desde appsettings.json.
Ejemplo
JwtSettings.cs
Debe incluir:
• secret key
• issuer
• audience
• expiration minutes
11. Program.cs
Es el punto de arranque de la API.
Qué configurará
• controllers
• swagger
• api versioning
• cors
• jwt
• dbcontext
• DI de las capas 2 y 3
• middleware global
• routing
Principios arquitectónicos aplicados en
esta capa
1. Separación de responsabilidades
La API solo expone y orquesta HTTP.
No debe hacer trabajo de negocio.
2. Seguridad por diseño
JWT, HTTPS, autorización, CORS y sanitización deben pensarse desde el inicio.
3. Escalabilidad
El versionamiento y la estandarización de respuestas permiten crecer sin caos.
4. Mantenibilidad
La configuración en extensiones y middleware evita que Program.cs se vuelva
inmanejable.
Diagrama de flujo de una petición
```
Caso: login
```
POST /api/v1/auth/login
↓
AuthController
↓
IAuthService
↓
AuthService
↓
UnitOfWork / UsuarioAppRepository
↓
Usuario validado
↓
Generar JWT
↓
LoginResponse con token
```
Caso: crear cliente
```
POST /api/v1/clientes
↓
ClientesController
↓
JWT válido + rol autorizado
↓
IClienteService
↓
ClienteService
↓
IClienteDataService
↓
UnitOfWork / ClienteRepository
↓
SQL Server / base de datos
↓
ClienteResponse
Diagrama por capas con la API incluida
Seguridad que debe contemplar esta
capa
La API V1 debe quedar preparada para:
• autenticación JWT
• autorización por roles
• validación de entrada
• sanitización básica
• uso de HTTPS
• respuestas de error seguras
• no exponer trazas internas al cliente
Validación y sanitización en la API
Validación
La validación funcional principal ya vive en la capa 3, pero la API debe:
• verificar modelos nulos
• recibir correctamente JSON
• devolver 400 Bad Request cuando corresponda
Sanitización
En V1 haremos una sanitización básica:
• trim de cadenas
• evitar cadenas vacías con espacios
• no confiar en texto entrante sin validación
La sanitización más robusta la podemos reforzar en V2.