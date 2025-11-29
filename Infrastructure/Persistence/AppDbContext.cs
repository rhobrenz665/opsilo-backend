using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly TenantContext _tenant;

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

            // Apply tenant filter globally for tenant-scoped entities
            modelBuilder.Entity<TenantScopedEntity>()
                        .HasQueryFilter(e => e.TenantId == _tenant.TenantId);
        }
    }
}
