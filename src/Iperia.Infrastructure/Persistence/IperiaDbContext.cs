using Iperia.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Iperia.Infrastructure.Persistence;

public class IperiaDbContext : DbContext
{
    public IperiaDbContext(DbContextOptions<IperiaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Toma todas las clases IEntityTypeConfiguration<T> de Persistence/Configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IperiaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
