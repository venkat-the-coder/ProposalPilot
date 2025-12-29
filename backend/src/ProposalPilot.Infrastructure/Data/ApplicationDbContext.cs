using Microsoft.EntityFrameworkCore;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<ProposalSection> ProposalSections { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ProposalAnalytics> ProposalAnalytics { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filters for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity has IsDeleted property
            if (entityType.ClrType.GetProperty("IsDeleted") != null)
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false)),
                    parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields before saving
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = DateTime.UtcNow;
                        entity.UpdatedAt = DateTime.UtcNow;
                        // CreatedBy and UpdatedBy will be set by the caller
                        break;
                    case EntityState.Modified:
                        entity.UpdatedAt = DateTime.UtcNow;
                        // UpdatedBy will be set by the caller
                        break;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
