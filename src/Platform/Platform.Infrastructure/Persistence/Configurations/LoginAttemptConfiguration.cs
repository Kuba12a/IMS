using Platform.Domain.Models.Identities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Platform.Infrastructure.Persistence.Configurations;

public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.ToTable("login_attempts");
        
        builder.Property(la => la.Id).ValueGeneratedNever();

        builder.OwnsOne(la => la.AuthCodeChallenge);
        builder.OwnsOne(la => la.TwoFactorEmailAuthenticationChallenge);
    }
}
