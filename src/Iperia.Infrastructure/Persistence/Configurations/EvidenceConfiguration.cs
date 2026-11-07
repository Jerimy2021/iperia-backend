using Iperia.Domain.Evidences;
using Iperia.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iperia.Infrastructure.Persistence.Configurations;

public class EvidenceConfiguration : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.ToTable("evidences");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(e => e.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(Evidence.StorageKeyMaxLength)
            .IsRequired();
        builder.HasIndex(e => e.StorageKey).IsUnique();

        builder.Property(e => e.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(Evidence.ContentTypeMaxLength)
            .IsRequired();

        builder.Property(e => e.SizeBytes).HasColumnName("size_bytes").IsRequired();

        builder.ComplexProperty(e => e.Location, location =>
        {
            location.Property(l => l.Latitude).HasColumnName("latitude").IsRequired();
            location.Property(l => l.Longitude).HasColumnName("longitude").IsRequired();
        });

        builder.Property(e => e.UploadedByUserId).HasColumnName("uploaded_by_user_id").IsRequired();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.HasIndex(e => e.CreatedAt);
    }
}
