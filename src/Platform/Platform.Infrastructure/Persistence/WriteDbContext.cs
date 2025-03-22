using System.Reflection;
using Platform.Domain.Models.Identities;
using Platform.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;

namespace Platform.Infrastructure.Persistence;

public class WriteDbContext : DbContext
{
    public DbSet<Identity> Identities { get; private set; }

    public WriteDbContext(DbContextOptions<WriteDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(InfrastructureConstants.Schema);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public new async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await base.SaveChangesAsync(cancellationToken);
    }
}
