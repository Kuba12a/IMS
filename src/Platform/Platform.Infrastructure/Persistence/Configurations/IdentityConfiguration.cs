using Platform.Domain.Models.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Platform.Infrastructure.Persistence.Configurations;

public class IdentityConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasIndex(i => i.Email).IsUnique();
        builder.HasIndex(i => i.EmailConfirmationTokenHash);
        builder.HasIndex(i => i.PasswordResetTokenHash);
    }
}
