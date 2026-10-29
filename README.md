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

La contraseña de PostgreSQL no se guarda en el repo: se define con user-secrets (sobrescribe `appsettings.Development.json`):

```bash
dotnet user-secrets set "ConnectionStrings:IperiaDb" "Host=localhost;Port=5432;Database=iperia;Username=postgres;Password=TU_PASSWORD" --project src/Iperia.Api
```

`Jwt:SecretKey` en `appsettings.Development.json` es solo para desarrollo. En producción se define con la variable de entorno `Jwt__SecretKey` (mínimo 32 caracteres).

## Endpoints de autenticación

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/register` | `{ fullName, email, password }` → 201 con el usuario (rol `Worker` por defecto) |
| POST | `/api/auth/login` | `{ email, password }` → 200 con `accessToken`, `expiresAt` y el usuario |
| GET | `/api/auth/me` | Requiere `Authorization: Bearer <accessToken>` → datos del usuario del token |

Password: 8 a 72 caracteres, con mayúsculas, minúsculas y números.
