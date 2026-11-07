using Iperia.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iperia.Infrastructure.Persistence.Configurations;

public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("revoked_tokens");

        builder.HasKey(t => t.TokenId);
        builder.Property(t => t.TokenId).HasColumnName("token_id").HasMaxLength(64).ValueGeneratedNever();

        builder.Property(t => t.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.HasIndex(t => t.ExpiresAt);
    }
}
