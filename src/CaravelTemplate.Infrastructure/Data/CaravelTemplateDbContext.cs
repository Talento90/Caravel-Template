using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Entities;
using CaravelTemplate.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CaravelTemplate.Infrastructure.Data
{
    public class CaravelTemplateDbContext : DbContext, IUnitOfWork
    {
        public const string DefaultSchema = "CaravelTemplate";

        public DbSet<Book> Books { get; set; } = null!;

        public CaravelTemplateDbContext(DbContextOptions<CaravelTemplateDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateDbContext).Assembly);
            
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
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Entity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((Entity)entityEntry.Entity).UpdatedAtUtc = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((Entity)entityEntry.Entity).CreatedAtUtc = DateTime.UtcNow;
                }
            }
        }
    }
}