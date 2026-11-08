# Despliegue en AWS Lambda (Serverless Framework)

Estado: **empaquetado listo, despliegue real pendiente de una cuenta de AWS con credenciales.** Nada de esto se probó todavía dentro de AWS; lo verificado está en la sección [Qué se verificó](#qué-se-verificó).

## Qué cambió en el código

- `Iperia.Api` usa `AddAWSLambdaHosting(LambdaEventSource.HttpApi)` (paquete `Amazon.Lambda.AspNetCoreServer.Hosting`). Dentro de Lambda la app atiende los eventos de API Gateway; **fuera de Lambda no hace nada**, así que `dotnet run`, los tests y Docker funcionan igual que antes. Los controllers no se tocaron.
- `appsettings.Development.json` no se incluye en el paquete publicado (lleva secretos de desarrollo local).
- `serverless.yml` en la raíz define la función Lambda (runtime `dotnet10`, `arm64`) y la HTTP API de API Gateway con las rutas `/` y `/{proxy+}`.
- El CI tiene un job `package` (separado de las pruebas y solo si estas pasan) que publica y comprime la API y la sube como artefacto `iperia-api-lambda`. No despliega.

## Empaquetar localmente

```bash
dotnet publish src/Iperia.Api -c Release -r linux-arm64 --self-contained false -o artifacts/publish
cd artifacts/publish && zip -r ../iperia-api.zip .
```

En PowerShell, en vez de `zip`: `Compress-Archive -Path * -DestinationPath ../iperia-api.zip -Force`.

## Desplegar

Requisitos: cuenta de AWS, credenciales configuradas (`aws configure` o SSO), Node.js y Serverless Framework. La versión 4 del framework pide iniciar sesión con una cuenta de Serverless y tiene condiciones de licencia propias: **revisar sus requisitos vigentes antes de adoptarla** (si no convienen, el mismo despliegue se puede hacer con AWS SAM o CDK).

```bash
export IPERIA_DB_CONNECTION_STRING="Host=...;Database=iperia;Username=...;Password=..."
export IPERIA_JWT_SECRET_KEY="...(mínimo 32 caracteres)..."
npx serverless deploy --stage dev
```

Variables que lee `serverless.yml` al desplegar (nunca se escriben en el repo, que es público):

| Variable de entorno | Se convierte en | Obligatoria |
|---|---|---|
| `IPERIA_DB_CONNECTION_STRING` | `ConnectionStrings__IperiaDb` | Sí |
| `IPERIA_JWT_SECRET_KEY` | `Jwt__SecretKey` | Sí |
| `IPERIA_JWT_ISSUER` | `Jwt__Issuer` | No (`iperia-api`) |
| `IPERIA_JWT_AUDIENCE` | `Jwt__Audience` | No (`iperia-clients`) |

`ASPNETCORE_ENVIRONMENT` se fija al nombre del stage (`dev`, `prod`...). Como no es `Development`, en AWS no se expone Swagger ni se crea el administrador de prueba.

## PostgreSQL en AWS (propuesta a validar)

Lambda abre muchas conexiones cortas y puede agotar las de PostgreSQL. Propuesta:

1. **Amazon RDS for PostgreSQL** (por ejemplo `db.t4g.micro` en dev) **detrás de RDS Proxy**, que agrupa las conexiones de todas las instancias de Lambda.
2. La función Lambda en **subredes privadas de la misma VPC** que la base. Para llegar a S3 sin NAT se usa un VPC endpoint de tipo Gateway (sin costo); para otros servicios (por ejemplo Secrets Manager) hace falta un endpoint de interfaz o NAT.
3. En la cadena de conexión, un `Maximum Pool Size` pequeño (por ejemplo 5), porque cada instancia de Lambda atiende una petición a la vez.
4. Alternativas si el costo o la VPC pesan: Aurora Serverless v2, o un PostgreSQL gestionado externo (Neon, Supabase), que no exige poner Lambda dentro de una VPC.

Pendiente de decidir con el equipo: cuál de las dos rutas se usa y quién crea la cuenta y la base.

## Migraciones y datos iniciales

- Las migraciones de EF Core **no se aplican al arrancar la función**. Se aplican como paso del despliegue con un bundle (`dotnet ef migrations bundle`) o a mano contra la base.
- El administrador inicial (`SeedAdmin`) solo se crea en `Development`. Para el primer despliegue hay que crear el primer Admin por otra vía (queda como tarea pendiente).

## Pendiente

- Desplegar de verdad a una cuenta de AWS (falta cuenta y credenciales) y medir el **arranque en frío**, que todavía no se midió en Lambda. Opciones a evaluar si es lento: `PublishReadyToRun` y más memoria.
- Cuando se mergee la subida de evidencias (PR #24): agregar al `serverless.yml` `S3__BucketName` y `S3__Region`, el bucket y el permiso IAM `s3:PutObject` / `s3:DeleteObject` sobre `evidencias/*` para el rol de la función. En AWS no se usan claves: el rol IAM las reemplaza.
- Cuando se mergee IPERC (PR #25): ese PR ya copia `config/iperc_matrix.json` junto a la API al publicar, así que viajará en el paquete; conviene comprobarlo con un `dotnet publish` después del merge.
- Secrets Manager en lugar de variables de entorno del Lambda (paso posterior, como indica el issue).
- Actualizar la página "Roadmap AWS Serverless" de la wiki con estas decisiones (la wiki es un repositorio aparte; falta publicarlo).

## Qué se verificó

- `dotnet build`, `dotnet test` y `dotnet run`: sin cambios de comportamiento (health 200, login inválido 401, Swagger 200 en Development).
- `dotnet publish -r linux-arm64` genera el paquete (zip de ~3,6 MB) con `Iperia.Api.dll`, `Iperia.Api.runtimeconfig.json` y las dependencias de Lambda, y **sin** `appsettings.Development.json`.
- **No verificado:** ejecución dentro de Lambda real o de un emulador, `serverless deploy`, y la sintaxis del `serverless.yml` contra el framework.
