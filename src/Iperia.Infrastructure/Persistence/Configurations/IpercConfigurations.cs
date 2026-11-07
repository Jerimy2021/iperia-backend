using Iperia.Domain.Iperc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iperia.Infrastructure.Persistence.Configurations;

public class PuestoConfiguration : IEntityTypeConfiguration<Puesto>
{
    public void Configure(EntityTypeBuilder<Puesto> builder)
    {
        builder.ToTable("puestos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(Puesto.NombreMaxLength).IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasMany(p => p.Tareas)
            .WithOne()
            .HasForeignKey("PuestoId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Tareas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.ToTable("tareas");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(t => t.Nombre).HasColumnName("nombre").HasMaxLength(Tarea.NombreMaxLength).IsRequired();
        builder.Property<Guid>("PuestoId").HasColumnName("puesto_id");

        builder.HasMany(t => t.Actividades)
            .WithOne()
            .HasForeignKey("TareaId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Actividades).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ActividadConfiguration : IEntityTypeConfiguration<Actividad>
{
    public void Configure(EntityTypeBuilder<Actividad> builder)
    {
        builder.ToTable("actividades");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(a => a.Nombre).HasColumnName("nombre").HasMaxLength(Actividad.NombreMaxLength).IsRequired();
        builder.Property<Guid>("TareaId").HasColumnName("tarea_id");

        builder.HasMany(a => a.Peligros)
            .WithOne()
            .HasForeignKey("ActividadId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(a => a.Peligros).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PeligroConfiguration : IEntityTypeConfiguration<Peligro>
{
    public void Configure(EntityTypeBuilder<Peligro> builder)
    {
        builder.ToTable("peligros");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.Descripcion).HasColumnName("descripcion").HasMaxLength(Peligro.DescripcionMaxLength).IsRequired();
        builder.Property<Guid>("ActividadId").HasColumnName("actividad_id");

        builder.HasOne(p => p.Riesgo)
            .WithOne()
            .HasForeignKey<Riesgo>("PeligroId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RiesgoConfiguration : IEntityTypeConfiguration<Riesgo>
{
    public void Configure(EntityTypeBuilder<Riesgo> builder)
    {
        builder.ToTable("riesgos");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(r => r.Probabilidad).HasColumnName("probabilidad").IsRequired();
        builder.Property(r => r.Severidad).HasColumnName("severidad").IsRequired();
        builder.Property(r => r.Valor).HasColumnName("valor").IsRequired();
        builder.Property<Guid>("PeligroId").HasColumnName("peligro_id");
    }
}
