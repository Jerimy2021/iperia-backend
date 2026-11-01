# iperia-backend

API de IperIA: autenticación, evidencias, IPERC, órdenes de trabajo, auditoría y notificaciones.

- .NET 8, arquitectura hexagonal (ver `src/Iperia.Domain`, `Iperia.Application`, `Iperia.Infrastructure`, `Iperia.Api`).
- PostgreSQL vía EF Core. Objetivo de despliegue: serverless en AWS (hoy el código corre como Web API clásico; ver página "Roadmap AWS Serverless" en la wiki del repo).
- Documentación extendida en la Wiki de GitHub: Arquitectura, Convenciones de Git, Entorno de desarrollo, Roadmap AWS Serverless.

## Arquitectura: reglas de dependencia

`Domain` no depende de nada. `Application` depende solo de `Domain` (puertos en `Ports/In` y `Ports/Out`, nunca implementaciones concretas). `Infrastructure` y `Api` dependen de `Application`, nunca al revés.

`tests/Iperia.Tests/ArchitectureTests.cs` valida esto automáticamente. Si un cambio rompe esta regla, el test falla y el PR no debe mergearse — no se debe silenciar ni borrar ese test para hacer pasar el build.

## Flujo de Git (obligatorio)

- `main` — producción, protegida. Solo recibe merges desde `develop`.
- `develop` — integración, protegida, rama por defecto del repo. Toda rama nueva sale de aquí, nunca de otra rama feature.
- Prefijos: `feat/`, `fix/`, `chore/`, `refactor/`.
- Commits en formato [Conventional Commits](https://www.conventionalcommits.org/): `<tipo>(<scope>): <descripción>`.
- Todo cambio entra por Pull Request contra `develop`, con la plantilla de `.github/pull_request_template.md`. Requiere 1 review aprobado (code owner) y CI en verde — está forzado por branch protection, no es opcional.
- PRs chicos y de una sola cosa. No mezclar varias features en una rama.

Detalle completo en `CONTRIBUTING.md` y en la página "Convenciones de Git" de la wiki.

## Antes de dar por terminado un cambio

```
dotnet build
dotnet test
```

Ambos deben pasar en limpio. El pipeline de CI (`.github/workflows/ci.yml`) corre lo mismo en cada PR — si falla localmente, va a fallar en CI.

## Entorno

El repo trae `flake.nix` + `.envrc` (Nix + direnv): .NET 8 SDK, `gh`, AWS CLI v2, `psql`. `nix develop` (o `direnv allow`) para activar. `docker compose up -d` levanta PostgreSQL local en `localhost:5434`.

## Seguridad

- El repo es público. Nunca commitear credenciales, connection strings reales ni secretos de producción.
- `appsettings.Development.json` solo lleva secretos de desarrollo local explícitamente marcados como tales (p. ej. `dev-only-...`), que no funcionan fuera de `localhost`. `appsettings.json` (el de producción) debe mantenerse con los campos de secretos vacíos; en AWS esos valores vienen de variables de entorno / Secrets Manager, nunca del repo.
