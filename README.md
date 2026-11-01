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

La cadena de conexión de desarrollo está en `src/Iperia.Api/appsettings.Development.json` (`ConnectionStrings:IperiaDb`).
