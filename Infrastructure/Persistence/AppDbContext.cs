using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Security;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly TenantContext? _tenant;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options, TenantContext tenant)
            : base(options)
        {
            _tenant = tenant;
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Technician> Technicians => Set<Technician>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(TenantScopedEntity).IsAssignableFrom(entityType.ClrType))
                {
                    
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(TenantScopedEntity.TenantId))
                        .HasDefaultValue(Guid.Empty);

                   
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

            //User roles many to many
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            //RolePermissions  many to many
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            //RefreshToken one to many
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //UserProfile one to one
            modelBuilder.Entity<User>()
               .HasOne(u => u.Profile)
               .WithOne()
               .HasForeignKey<User>(u => u.ProfileId)
               .OnDelete(DeleteBehavior.SetNull);

            //Unique indexes
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
            modelBuilder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();
        }
    }
}
