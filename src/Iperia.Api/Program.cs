using Amazon.Lambda.AspNetCoreServer.Hosting;
using Iperia.Api.Authentication;
using Iperia.Api.ExceptionHandling;
using Iperia.Application;
using Iperia.Infrastructure;
using Iperia.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Dentro de AWS Lambda (API Gateway HTTP API) la app responde eventos de Lambda en vez de abrir Kestrel.
// Fuera de Lambda (dotnet run, tests, Docker) no hace nada: el flujo local no cambia.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
// Devuelve ProblemDetails también en 401/403, que por defecto van con cuerpo vacío.
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.Services.SeedAdminAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
