using System.Reflection;
using Platform.Domain.Models.Identities;
using Platform.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;

namespace Platform.Infrastructure.Persistence;

internal class ReadDbContext : DbContext
{
    public DbSet<Identity> Identities { get; private set; }

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(InfrastructureConstants.Schema);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
