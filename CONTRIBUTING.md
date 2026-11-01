# Contribuir a iperia-backend

## Ramas

- `main` — producción. Solo recibe merges desde `development` vía PR, nunca se le hacen commits directos.
- `development` — rama de integración. Todas las features se integran aquí primero.

```
feat/<descripcion>       nueva funcionalidad
fix/<descripcion>        corrección de bug
chore/<descripcion>      mantenimiento, dependencias, configuración
refactor/<descripcion>   cambio de estructura interna sin alterar comportamiento
```

Toda rama nueva sale de `development` actualizado (`git checkout development && git pull && git checkout -b feat/mi-feature`). No crear una rama a partir de otra rama feature: si tu trabajo depende de algo que aún no está en `development`, espera a que se mergee o avísalo para priorizar ese PR.

## Mensajes de commit

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

```
<tipo>(<scope opcional>): <descripción corta en imperativo>
```

Tipos: `feat`, `fix`, `chore`, `refactor`, `test`, `docs`.
Scope sugerido por dominio: `(auth)`, `(evidencias)`, `(iperc)`, `(ordenes-trabajo)`, `(auditoria)`, `(notificaciones)`, `(db)`.

Ejemplos:
- `feat(auth): agregar login con JWT`
- `fix(auth): corregir validación de rol en UsersController`
- `chore(db): actualizar migración de EF Core`

## Pull Requests

- Todo PR va contra `development` (o `development` → `main` para releases).
- Usa la plantilla de `.github/pull_request_template.md`.
- Requiere al menos 1 review aprobado y que pasen los checks de CI antes de poder mergear.
- PRs chicos y enfocados en una sola cosa. Si tu rama ya tiene varios commits de features distintas, sepáralos en PRs distintos.

## Antes de abrir el PR

```
dotnet build
dotnet test
```

Ambos deben pasar en local. El pipeline de CI corre lo mismo en cada PR.

## Entorno de desarrollo

El repo trae `flake.nix` + `.envrc` (Nix + direnv) con .NET 8 SDK, AWS CLI, GitHub CLI (`gh`) y cliente de PostgreSQL. Al entrar al directorio con direnv activado, el entorno se carga solo. Sin direnv: `nix develop`.

Base de datos local: `docker compose up -d` levanta PostgreSQL en `localhost:5434`.
