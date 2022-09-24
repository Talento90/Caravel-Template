using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caravel.ApplicationContext;
using Caravel.Entities;
using CaravelTemplate.Core.Data;
using CaravelTemplate.Entities;
using CaravelTemplate.Events;
using CaravelTemplate.Infrastructure.Authentication;
using CaravelTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using JsonSerializer = Caravel.Http.JsonSerializer;

namespace CaravelTemplate.Infrastructure.Data
{
    public class CaravelTemplateTemplateDbContext : IdentityDbContext<Identity.User, Role, Guid>,
        ICaravelTemplateDbContext
    {
        private readonly IAppContextAccessor _contextAccessor;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public CaravelTemplateTemplateDbContext(
            DbContextOptions<CaravelTemplateTemplateDbContext> options,
            IAppContextAccessor appContextAccessor
        ) : base(options)
        {
            _contextAccessor = appContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateTemplateDbContext).Assembly);

            NormalizeTableNames(modelBuilder);
            ApplyUtcDateConverter(modelBuilder);
        }

        public override int SaveChanges()
        {
            AuditEntities();
            SaveDomainEvents();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AuditEntities();
            SaveDomainEvents();
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
                .Where(e => e.Entity is IAuditable && 
                            e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

            foreach (var entityEntry in entries)
            {
                var entity = (IEntity) entityEntry.Entity;
                
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

        private void SaveDomainEvents()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAggregateRoot && e.State is 
                    EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entityEntry in entries.Where(e => e.State == EntityState.Added))
            {
                if (entityEntry.Entity is not IAggregateRoot entity)
                    continue;

                entity.AddEvent(new CreateEntityEvent(entity));
            }

            // Save all events
            foreach (var entry in entries)
            {
                if (entry.Entity is not IAggregateRoot entity)
                    continue;

                foreach (var domainEvent in entity.Events)
                {
                    Events.Add(new Event(domainEvent.Name, JsonSerializer.Serialize(domainEvent, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })));
                }

                entity.ClearEvents();
            }
        }

        private static void NormalizeTableNames(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();

                if (tableName is not null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}