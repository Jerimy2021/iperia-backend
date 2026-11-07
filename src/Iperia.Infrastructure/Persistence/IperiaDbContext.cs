using Iperia.Domain.Iperc;
using Iperia.Domain.Users;
using Iperia.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Iperia.Infrastructure.Persistence;

public class IperiaDbContext : DbContext
{
    public IperiaDbContext(DbContextOptions<IperiaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    public DbSet<Puesto> Puestos => Set<Puesto>();
    public DbSet<Tarea> Tareas => Set<Tarea>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<Peligro> Peligros => Set<Peligro>();
    public DbSet<Riesgo> Riesgos => Set<Riesgo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Toma todas las clases IEntityTypeConfiguration<T> de Persistence/Configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IperiaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
