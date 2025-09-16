using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUser? _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(AppDbContext).GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    var generic = method!.MakeGenericMethod(entityType.ClrType);
                    generic.Invoke(null, new object[] { modelBuilder });
                }
            }
        }

        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : Entity
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var now = DateTime.UtcNow;
            var userId = _currentUser?.UserId;

            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.CreatedDate == default)
                        entry.Entity.CreatedDate = now;
                    if (!entry.Entity.CreatedById.HasValue)
                        entry.Entity.CreatedById = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = now;
                    entry.Entity.ModifiedById = userId;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.ModifiedDate = now;
                    entry.Entity.ModifiedById = userId;
                }
            }
        }
    }
}
