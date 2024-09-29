using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Adapter.Identity;

public class IdentityDbContext : IdentityDbContext<User, Role, Guid>
{
    public const string Schema = "identity";
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options): base(options)
    {
    } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schema);
        NormalizeTableNames(modelBuilder);
    }

    private static void NormalizeTableNames(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();

            if (tableName is not null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName[6..]);
            }
        }
    }
}