using System.Threading;
using System.Threading.Tasks;
using Caravel.ApplicationContext;
using Caravel.Entities;
using Caravel.EntityFramework;
using CaravelTemplate.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Infrastructure.Data
{
    public class CaravelTemplateTemplateDbContext : DbContext
    {
        private readonly IApplicationContextAccessor _contextAccessor;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<EntityEvent> Events { get; set; } = null!;
        public CaravelTemplateTemplateDbContext(
            DbContextOptions<CaravelTemplateTemplateDbContext> options,
            IApplicationContextAccessor appContextAccessor
        ) : base(options)
        {
            _contextAccessor = appContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateTemplateDbContext).Assembly);
            modelBuilder.ApplyUtcDateConverter();
        }

        public override int SaveChanges()
        {
            this.AuditEntities(_contextAccessor);
            this.CreateAndSaveDomainEvents<Book>(Events);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            this.AuditEntities(_contextAccessor);
            this.CreateAndSaveDomainEvents<Book>(Events);
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}