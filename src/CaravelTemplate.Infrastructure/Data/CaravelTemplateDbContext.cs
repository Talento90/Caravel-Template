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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateDbContext).Assembly);

            
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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
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

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}