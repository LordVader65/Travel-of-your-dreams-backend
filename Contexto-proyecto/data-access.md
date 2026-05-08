La presente guía original es conceptual y usa Clientes + SQL Server como ejemplo. Para este proyecto, el agente debe implementar AtraccionesTuristicas.Backend.LA.DataAccess con PostgreSQL, Npgsql, migraciones EF Core, modelo sin esquema y la estructura oficial indicada.
```
Capa: Acceso a Datos.
```
Esta capa es la que se conecta con SQL Server y se encarga de la persistencia real del
microservicio. Su objetivo no es tomar decisiones de negocio, sino guardar, consultar
y actualizar datos de forma ordenada, segura y mantenible.
Diagrama de componentes de la capa de Acceso a Datos
Vista arquitectónica de la capa
Qué busca esta capa
La capa de acceso a datos debe resolver estas necesidades:
• conectarse a la base de datos
• mapear tablas a clases
• ejecutar operaciones CRUD
• encapsular consultas
• abstraer el motor de persistencia
• facilitar pruebas y mantenimiento
• servir de base para auditoría, seguridad y trazabilidad
Componentes que vamos a usar
1. ClientesDbContext
Qué es
Es el componente central de EF Core. Representa la sesión con la base de datos y
permite trabajar con las tablas como objetos.
Qué hace
• administra la conexión con SQL Server
• expone las colecciones DbSet
• aplica configuraciones de mapeo
```
• permite guardar cambios con SaveChangesAsync()
```
Ejemplo conceptual
public class ClientesDbContext : DbContext
```
{
```
```
public DbSet<ClienteEntity> Clientes { get; set; }
```
```
public DbSet<UsuarioAppEntity> Usuarios { get; set; }
```
```
public DbSet<RolEntity> Roles { get; set; }
```
```
public DbSet<AuditoriaEntity> Auditorias { get; set; }
```
```
protected override void OnModelCreating(ModelBuilder modelBuilder)
```
```
{
```
```
modelBuilder.ApplyConfiguration(new ClienteConfiguration());
```
```
modelBuilder.ApplyConfiguration(new
```
```
UsuarioAppConfiguration());
```
```
modelBuilder.ApplyConfiguration(new RolConfiguration());
```
```
modelBuilder.ApplyConfiguration(new AuditoriaConfiguration());
```
```
}
```
```
}
```
Por qué es importante
Porque centraliza toda la infraestructura de persistencia.
Sin este componente, cada clase tendría que manejar manualmente conexiones y SQL.
2. ClienteEntity
Qué es
Es la representación en código de la tabla crm.Cliente.
Qué hace
Define las propiedades que corresponden a los campos físicos de la tabla.
Ejemplo conceptual
public class ClienteEntity
```
{
```
```
public long ClienteId { get; set; }
```
```
public Guid ClienteGuid { get; set; }
```
```
public string CodigoCliente { get; set; }
```
```
public string TipoCliente { get; set; }
```
```
public string EstadoCliente { get; set; }
```
```
public string Nombres { get; set; }
```
```
public string Apellidos { get; set; }
```
```
public string RazonSocial { get; set; }
```
```
public string TipoIdentificacion { get; set; }
```
```
public string NumeroIdentificacion { get; set; }
```
```
public string CorreoElectronico { get; set; }
```
```
public bool EsEliminado { get; set; }
```
```
}
```
Por qué es importante
Porque convierte la estructura relacional en una estructura orientada a objetos, que
luego será usada por repositorios y servicios.
3. Configuration de entidades
Qué es
Son clases que definen el mapeo exacto entre entidades y tablas en SQL Server.
Qué hacen
• especifican la tabla y esquema
• definen claves primarias
• restricciones
• longitudes de columnas
• índices
• nombres exactos de columnas
• relaciones entre tablas
Ejemplo conceptual
public class ClienteConfiguration :
IEntityTypeConfiguration<ClienteEntity>
```
{
```
```
public void Configure(EntityTypeBuilder<ClienteEntity> builder)
```
```
{
```
```
builder.ToTable("Cliente", "crm");
```
```
builder.HasKey(x => x.ClienteId);
```
```
builder.Property(x => x.CodigoCliente)
```
```
.HasMaxLength(30)
```
```
.IsRequired();
```
```
builder.Property(x => x.TipoCliente)
```
```
.HasMaxLength(20)
```
```
.IsRequired();
```
```
builder.HasIndex(x => x.NumeroIdentificacion)
```
```
.IsUnique();
```
```
}
```
```
}
```
Por qué es importante
Porque separa la entidad del detalle técnico de persistencia.
Eso vuelve el diseño más limpio y profesional.
4. IClienteRepository
Qué es
Es la interfaz que define las operaciones que la capa superior podrá pedir sobre clientes.
Qué hace
Declara contratos como:
• obtener por id
• obtener por identificación
• listar
• insertar
• actualizar
• eliminar lógicamente
Ejemplo conceptual
public interface IClienteRepository
```
{
```
```
Task<ClienteEntity?> ObtenerPorIdAsync(long clienteId);
```
```
Task<ClienteEntity?> ObtenerPorIdentificacionAsync(string
```
```
numeroIdentificacion);
```
```
Task<IReadOnlyList<ClienteEntity>> ListarAsync();
```
```
Task AgregarAsync(ClienteEntity cliente);
```
```
void Actualizar(ClienteEntity cliente);
```
```
}
```
Por qué es importante
Porque desacopla la definición del comportamiento de su implementación.
La capa de negocio no depende de EF Core directamente, sino de contratos.
5. ClienteRepository
Qué es
Es la implementación concreta del repositorio de clientes.
Qué hace
Usa ClientesDbContext para ejecutar operaciones reales sobre la tabla Cliente.
Responsabilidades
• consultas básicas
• inserción
• actualización
• filtrado inicial
• persistencia estandarizada
Ejemplo conceptual
public class ClienteRepository : IClienteRepository
```
{
```
```
private readonly ClientesDbContext _context;
```
```
public ClienteRepository(ClientesDbContext context)
```
```
{
```
```
_context = context;
```
```
}
```
```
public async Task<ClienteEntity?> ObtenerPorIdAsync(long
```
```
clienteId)
```
```
{
```
return await _context.Clientes
```
.FirstOrDefaultAsync(x => x.ClienteId == clienteId &&
```
```
!x.EsEliminado);
```
```
}
```
```
public async Task AgregarAsync(ClienteEntity cliente)
```
```
{
```
```
await _context.Clientes.AddAsync(cliente);
```
```
}
```
```
public void Actualizar(ClienteEntity cliente)
```
```
{
```
```
_context.Clientes.Update(cliente);
```
```
}
```
```
}
```
Por qué es importante
Porque encapsula la persistencia y evita que otras capas manipulen directamente el
contexto o SQL.
6. ClienteQueryRepository o consultas especiales
Qué es
Es un componente opcional, pero muy recomendable cuando existen consultas
complejas o de alto rendimiento.
Qué hace
```
Ejecuta:
```
• consultas optimizadas
• búsquedas paginadas
• filtros avanzados
• reportes
• joins
• búsquedas por texto
Puede usar:
• EF Core
• Dapper
• SQL parametrizado
Cuándo usarlo
Cuando el repositorio CRUD ya no es suficiente y necesitas consultas especializadas.
Ejemplo de uso
• buscar clientes por nombres, ciudad, estado y tipo
• listar clientes paginados
• buscar por coincidencia parcial
• obtener dashboard de clientes activos/inactivos
Por qué es importante
Porque separa claramente:
• operaciones transaccionales
• operaciones de consulta compleja
Esto mejora rendimiento y organización.
7. AuditoriaEntity
Qué es
Una entidad destinada a registrar acciones importantes sobre los datos.
Qué guarda
• usuario
• acción
• fecha
• tabla afectada
• registro afectado
• valores anteriores y nuevos
• IP o servicio origen
Ejemplo
public class AuditoriaEntity
```
{
```
```
public long AuditoriaId { get; set; }
```
```
public string Tabla { get; set; }
```
```
public string Accion { get; set; }
```
```
public string Usuario { get; set; }
```
```
public DateTime FechaUtc { get; set; }
```
```
public string RegistroId { get; set; }
```
```
public string DatosAnteriores { get; set; }
```
```
public string DatosNuevos { get; set; }
```
```
}
```
Por qué es importante
Porque el microservicio tendrá autenticación y autorización, así que no basta con
guardar datos: también hay que poder rastrear quién hizo qué.
8. UsuarioAppEntity y RolEntity
Qué son
Son las entidades de autenticación y autorización a nivel aplicación.
Para qué sirven
Aunque ya creamos logins en SQL Server, para la API con JWT conviene manejar
usuarios de aplicación, por ejemplo:
• admin
• vendedor
Y sus roles:
• ADMIN
• VENDEDOR
Qué permiten
• autenticación vía API
• emisión de JWT
• autorización por rol
• manejo de usuarios activos/inactivos
• hashing de contraseñas
Importante
Los usuarios de SQL Server sirven para la infraestructura.
Los usuarios de aplicación sirven para la seguridad del sistema.
9. RepositoryBase
Qué es
Una clase base opcional para compartir lógica común entre repositorios.
Qué puede incluir
• acceso al DbContext
• métodos genéricos
• utilitarios comunes
• operaciones base reutilizables
Ventaja
Evita duplicación.
10. PagedResult
Qué es
Un modelo auxiliar para devolver consultas paginadas.
Qué contiene
• lista de resultados
• total de registros
• página actual
• tamaño de página
Por qué es importante
Porque desde el inicio debemos diseñar el acceso a datos pensando en listas grandes.
No es buena práctica devolver siempre tablas completas.

Relación entre componentes
Flujo interno
Capa Gestión de Datos
↓
IClienteRepository
↓
ClienteRepository
↓
ClientesDbContext
↓
Entity Configuration + Entities
↓
SQL Server / base de datos a usar

Diagrama más detallado
Explicación de la arquitectura de esta capa
Principio 1: separación de responsabilidades
Una arquitectura profesional separa:
• entidad
• configuración
• repositorio
• contexto
• consultas especiales
Eso evita que una sola clase haga todo.
Principio 2: bajo acoplamiento
La lógica de negocio no debe depender de SQL Server directamente.
Debe depender de interfaces.
Por eso usamos IClienteRepository.
Principio 3: alta cohesión
Cada componente debe hacer una sola cosa bien:
• la entidad representa datos
• la configuración mapea
• el repositorio persiste
• el contexto conecta
• el query repository consulta
Principio 4: extensibilidad
Hoy tendremos Cliente, pero mañana podríamos agregar:
• direcciones
• teléfonos
• usuarios
• auditoría
• refresh tokens
Si la capa está bien diseñada, crecerá sin romperse.
Principio 5: seguridad por diseño
Aunque esta capa no expone HTTP, sí debe prepararse para soportar:
• usuarios autenticados
• auditoría
• borrado lógico
• roles
• trazabilidad
```
La seguridad no empieza en la API; empieza desde el modelo de datos.
```
Qué tecnologías usaremos aquí
Principal
• ASP.NET Core / .NET 8
• Entity Framework Core
• SQL Server
Opcional para consultas avanzadas
• Dapper
Qué NO debe hacer esta capa
Esta capa no debe:
• validar reglas de negocio complejas
• decidir si un usuario puede o no puede ejecutar una acción
• emitir JWT
• manejar CORS
• exponer endpoints
• definir responses HTTP
Eso corresponde a otras capas.
Qué sí debe hacer muy bien
Sí debe:
• persistir correctamente
• mapear bien las tablas
• ejecutar consultas seguras
• usar parámetros
• respetar auditoría
• permitir borrado lógico
• soportar async/await
• estar lista para transacciones
Propuesta concreta de componentes mínimos para empezar
Para la primera versión del microservicio, yo usaría estos componentes mínimos:
Obligatorios
• ClientesDbContext
• ClienteEntity
• ClienteConfiguration
• IClienteRepository
• ClienteRepository
Muy recomendables desde ya
• UsuarioAppEntity
• RolEntity
• AuditoriaEntity
Opcionales para la siguiente iteración
• ClienteQueryRepository
• RepositoryBase
• PagedResult
Resultado esperado de esta capa
Cuando terminemos esta primera capa, deberíamos poder:
• conectarnos a SQL Server
• mapear la tabla crm.Cliente
• leer clientes
• insertar clientes
• actualizar clientes
• consultar por id e identificación
• dejar lista la base para autenticación y auditoría
Resumen
La capa de acceso a datos estará compuesta por:
• DbContext: punto central de conexión y persistencia
• Entities: representación de tablas
• Configurations: mapeo técnico de EF Core
• Repositories: acceso encapsulado a datos
• Query Repositories: consultas avanzadas
• Entidades de seguridad y auditoría: soporte para JWT, autenticación y
trazabilidad
Objetivo de esta fase
Construir la capa Microservicio.Clientes.DataAccess para que pueda:
• conectarse a SQL Server
• mapear la tabla crm.Cliente
• mapear usuarios y roles de aplicación
• dejar preparada la auditoría
• exponer repositorios para operaciones CRUD
• quedar alineada con la arquitectura de 4 capas
Arquitectura que vamos a materializar
ahora nos vamos a centrar en la capa de acceso a datos



