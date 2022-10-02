using Caravel.ApplicationContext;
using Caravel.Entities;
using Caravel.EntityFramework;
using CaravelTemplate.Identity.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Identity.Data;

public class CaravelTemplateIdentityDbContext : IdentityDbContext<User, Role, Guid>
{
    private readonly IAppContextAccessor _contextAccessor;
    public DbSet<EntityEvent> Events { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public CaravelTemplateIdentityDbContext(
        DbContextOptions<CaravelTemplateIdentityDbContext> options,
        IAppContextAccessor appContextAccessor
    ) : base(options)
    {
        _contextAccessor = appContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaravelTemplateIdentityDbContext).Assembly);
        NormalizeTableNames(modelBuilder);
        modelBuilder.ApplyUtcDateConverter();
    }

    public override int SaveChanges()
    {
        this.AuditEntities(_contextAccessor);
        this.CreateAndSaveDomainEvents<User>(Events);
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        this.AuditEntities(_contextAccessor);
        this.CreateAndSaveDomainEvents<User>(Events);
        return base.SaveChangesAsync(cancellationToken);
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