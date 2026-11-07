# iperia-backend
API de IperIA: auth, evidencias, IPERC, órdenes de trabajo, auditoría y notificaciones.

Stack: .NET 8 · ASP.NET Core Web API · PostgreSQL · Entity Framework Core · arquitectura hexagonal + DDD.

## Capas

Las dependencias siempre apuntan hacia adentro: `Api → Infrastructure → Application → Domain`.

| Proyecto | Rol hexagonal | Contenido |
|---|---|---|
| `Iperia.Domain` | Núcleo | Entidades, agregados, objetos de valor, reglas de negocio (`Common/`, `Exceptions/`). Sin dependencias externas. |
| `Iperia.Application` | Casos de uso + puertos | `Ports/In` (interfaces de casos de uso), `Ports/Out` (repositorios, `IUnitOfWork`, hasher, tokens…), `UseCases/`, `Common/Exceptions`. |
| `Iperia.Infrastructure` | Adaptadores de salida | EF Core + PostgreSQL (`Persistence/IperiaDbContext`, `Configurations/`, `Repositories/`), seguridad, servicios externos. |
| `Iperia.Api` | Adaptador de entrada | Controllers HTTP, manejo global de errores (`ExceptionHandling/`), `Program.cs` (composición de DI). |
| `tests/Iperia.Tests` | Pruebas | Tests unitarios y de arquitectura (verifican la regla de dependencias). |

## Comandos

```bash
dotnet build
dotnet test
dotnet run --project src/Iperia.Api          # Swagger: http://localhost:5135/swagger

# Migraciones (requiere: dotnet tool install --global dotnet-ef --version 8.0.11)
dotnet ef migrations add <Nombre> -p src/Iperia.Infrastructure -s src/Iperia.Api -o Persistence/Migrations
dotnet ef database update -p src/Iperia.Infrastructure -s src/Iperia.Api
```

## Configuración local

La base de datos de desarrollo corre en Docker (PostgreSQL 17 en el puerto **5434**, usuario `iperia`, password `iperia_dev`). `appsettings.Development.json` ya apunta a ella:

```bash
docker compose up -d                                                   # levanta PostgreSQL
dotnet ef database update -p src/Iperia.Infrastructure -s src/Iperia.Api   # crea las tablas
dotnet run --project src/Iperia.Api
```

Para usar otro PostgreSQL (por ejemplo uno instalado en tu equipo), sobrescribe la cadena con user-secrets sin tocar el repo:

```bash
dotnet user-secrets set "ConnectionStrings:IperiaDb" "Host=localhost;Port=5432;Database=iperia;Username=postgres;Password=TU_PASSWORD" --project src/Iperia.Api
```

`Jwt:SecretKey` en `appsettings.Development.json` es solo para desarrollo. En producción se define con la variable de entorno `Jwt__SecretKey` (mínimo 32 caracteres).

## Matriz IPERC

El modelo (`Puesto → Tarea → Actividad → Peligro → Riesgo`) vive en `Iperia.Domain/Iperc` y el cálculo en `IpercRiskCalculator` (`Iperia.Application/Iperc`). `riesgo = probabilidad × severidad`, y las escalas y umbrales se leen de **`config/iperc_matrix.json`**, no del código: cambiar el JSON cambia el resultado sin recompilar (el archivo se vuelve a leer cuando cambia). Para usar otro archivo: `Iperc:MatrixPath` (variable de entorno `Iperc__MatrixPath`).

Estado de la matriz:
- **Severidad:** confirmada (sin lesión 0, lesión no incapacitante 2, incapacitante temporal 4, incapacitante permanente 6, fatalidad 8).
- **Probabilidad y umbrales de nivel** (ACEPTABLE, MODERADO, IMPORTANTE, INTOLERABLE): **pendientes de confirmar con Ing. Industrial**; el archivo trae placeholders `TODO` y no se inventaron valores. Mientras estén vacíos, el cálculo responde que la matriz no está completa (`503` cuando exista el endpoint).

Al confirmarlos, se completan así (niveles en orden ascendente; cada uno empieza en `desde`, inclusive):

```json
"probabilidad": { "escala": { "nombre": 1 } },
"umbrales": { "niveles": [ { "nivel": "ACEPTABLE", "desde": 0 }, { "nivel": "MODERADO", "desde": 5 }, { "nivel": "IMPORTANTE", "desde": 10 }, { "nivel": "INTOLERABLE", "desde": 20 } ] }
```

(Los números del ejemplo son ilustrativos, no los reales.) Luego se actualiza el test `ArchivoReal_NoInventaProbabilidadNiUmbralesPendientes`.

## Endpoints de autenticación

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/register` | `{ fullName, email, password }` → 201 con el usuario (rol `Operario` por defecto) |
| POST | `/api/auth/login` | `{ email, password }` → 200 con `accessToken`, `expiresAt` y el usuario |
| GET | `/api/auth/me` | Requiere `Authorization: Bearer <accessToken>` → datos del usuario del token |

Password: 8 a 72 caracteres, con mayúsculas, minúsculas y números.

## Roles y permisos

Roles: `Admin`, `Operario` (por defecto al registrarse), `Supervisor`, `GerenciaEhs` y `Brigadista`.
`Admin` tiene los permisos de todos los demás roles, además de la gestión de usuarios.

| Permiso (política) | Admin | Operario | Supervisor | Gerencia EHS | Brigadista |
|---|:-:|:-:|:-:|:-:|:-:|
| Gestionar roles de usuarios (`AdminOnly`) | ✅ | ❌ | ❌ | ❌ | ❌ |
| Listar usuarios (`AdminOrSupervisor`) | ✅ | ❌ | ✅ | ❌ | ❌ |
| Aprobar órdenes de trabajo (`CanApproveWorkOrder`) | ✅ | ❌ | ✅ | ❌ | ❌ |
| Ver el dashboard gerencial (`CanViewManagementDashboard`) | ✅ | ❌ | ❌ | ✅ | ❌ |

Las políticas de aprobación de OT y dashboard gerencial ya existen; se aplicarán con `[Authorize(Policy = ...)]` cuando se creen esos módulos. Si cambia la matriz, se actualiza `Policies.cs` y esta tabla.

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| GET | `/api/users` | Admin, Supervisor | Lista los usuarios |
| PATCH | `/api/users/{id}/role` | Admin | `{ role }` → cambia el rol (un Admin no puede cambiar su propio rol) |

Sin token → **401**. Con token pero sin el rol necesario → **403**.

Para proteger un endpoint nuevo se usan las políticas de `src/Iperia.Api/Authentication/Policies.cs`:

```csharp
[Authorize(Policy = Policies.AdminOnly)]          // solo Admin
[Authorize(Policy = Policies.AdminOrSupervisor)]  // Admin o Supervisor
[Authorize(Policy = Policies.CanApproveWorkOrder)] // Admin o Supervisor
[Authorize(Policy = Policies.CanViewManagementDashboard)] // Admin o Gerencia EHS
[Authorize]                                       // cualquier usuario autenticado
```

El rol viaja dentro del token: después de cambiar el rol de alguien, esa persona debe volver a iniciar sesión.

**Admin inicial (desarrollo):** al arrancar en `Development` se crea `admin@iperia.local` / `Admin12345` (sección `SeedAdmin` de `appsettings.Development.json`) si todavía no existe.

## Postman

Importa `postman/IperIA-Auth.postman_collection.json`. Ejecuta *Iniciar sesión* (o *Iniciar sesión (Admin)*) y el token se guarda solo en `{{accessToken}}`. *Listar usuarios* guarda un `{{userId}}` para *Cambiar rol*.
