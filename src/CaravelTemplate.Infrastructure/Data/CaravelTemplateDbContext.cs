using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caravel.AppContext;
using Caravel.Entities;
using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CaravelTemplate.Infrastructure.Data
{
    public class CaravelTemplateDbContext : IdentityDbContext<User, Role, Guid>
    {
        public const string DefaultSchema = "CaravelTemplate";

        private readonly IAppContextAccessor _contextAccessor;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public CaravelTemplateDbContext(
            DbContextOptions<CaravelTemplateDbContext> options,
            IAppContextAccessor appContextAccessor
            ) : base(options)
        {
            _contextAccessor = appContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateDbContext).Assembly);

            NormalizeTableNames(modelBuilder);
            ApplyUtcDateConverter(modelBuilder);
        }
        

        public override int SaveChanges()
        {
            AuditEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private static void ApplyUtcDateConverter(ModelBuilder modelBuilder)
        {
            var utcDateTimeConverter = new ValueConverter<DateTime, DateTime>(
                d => d,
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            var nullableUtcDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                d => d, 
                d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType
                    .GetProperties()
                    .Where(p => p.Name.EndsWith("Utc"));
                
                foreach (var p in properties)
                {
                    if (p.ClrType == typeof(DateTime))
                    {
                        p.SetValueConverter(utcDateTimeConverter);
                    }
                    
                    if (p.ClrType == typeof(DateTime?))
                    {
                        p.SetValueConverter(nullableUtcDateTimeConverter);
                    }
                }
            }
        }

        private void AuditEntities()
        {
            var userId = _contextAccessor.Context.UserId;
            
            
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified
                    || e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {
                IEntity entity = (IEntity) entityEntry.Entity;

                if (entity != null)
                {
                    switch (entityEntry.State)
                    {
                        case EntityState.Added:
                        {
                            entity.Created = DateTime.UtcNow;
                    
                            if (userId.HasValue)
                            {
                                entity.CreatedBy = userId.Value;
                            }

                            break;
                        }
                        case EntityState.Modified:
                        case EntityState.Deleted:
                        {
                            entity.Updated = DateTime.UtcNow;
                
                            if (userId.HasValue)
                            {
                                entity.UpdatedBy = userId.Value;
                            }

                            break;
                        }
                    }
                }
            }
        }
        
        private static void NormalizeTableNames(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}