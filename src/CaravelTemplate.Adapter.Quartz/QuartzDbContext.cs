using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Adapter.Quartz;

public class QuartzDbContext : DbContext
{
    public const string Schema = "quartz";
    public QuartzDbContext(DbContextOptions<QuartzDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuartzDbContext).Assembly);
        modelBuilder.HasDefaultSchema("quartz");
        modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
    }
}