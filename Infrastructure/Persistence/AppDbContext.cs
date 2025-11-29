using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly TenantContext? _tenant;

        // Design-time constructor (used by EF Core migrations)
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Runtime constructor (used in this app)
        public AppDbContext(DbContextOptions<AppDbContext> options, TenantContext tenant)
            : base(options)
        {
            _tenant = tenant;
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Technician> Technicians => Set<Technician>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply default value for TenantId to avoid non-nullable issues at design-time
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(TenantScopedEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Default value for TenantId in database
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(TenantScopedEntity.TenantId))
                        .HasDefaultValue(Guid.Empty);

                    // Runtime query filter
                    if (_tenant != null)
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");
                        var property = Expression.Property(parameter, nameof(TenantScopedEntity.TenantId));
                        var tenantId = Expression.Constant(_tenant.TenantId);
                        var compare = Expression.Equal(property, tenantId);
                        var lambda = Expression.Lambda(compare, parameter);

                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                }
            }
        }
    }
}
