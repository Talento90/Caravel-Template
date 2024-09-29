using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.PostgreSql;
using CaravelTemplate.Adapter.Quartz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CaravelTemplate.Migrator.Extensions;

public static class DbContextExtensions
{
    public static void AddApplicationDbContext(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var options = configuration
                          .GetSection("PostgreSql")
                          .Get<PostgreSqlOptions>()
                      ?? throw new NullReferenceException(nameof(PostgreSqlOptions));

        services.AddDbContext<ApplicationDbContext>((dbContextBuilder) =>
        {
            dbContextBuilder.UseNpgsql(options.ConnectionString, optionBuilder =>
            {
                optionBuilder.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName,
                    schema: ApplicationDbContext.Schema);

                optionBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
    }
    
    public static void AddQuartzDbContext(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var options = configuration
                          .GetSection("Quartz")
                          .Get<QuartzOptions>()
                      ?? throw new NullReferenceException(nameof(QuartzOptions));

        services.AddDbContext<QuartzDbContext>((dbContextBuilder) =>
        {
            dbContextBuilder.UseNpgsql(options.ConnectionString, optionBuilder =>
            {
                optionBuilder.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName,
                    schema: QuartzDbContext.Schema);

                optionBuilder.MigrationsAssembly(typeof(QuartzDbContext).Assembly.FullName);
            });
        });
    }
    
    public static void AddIdentityDbContext(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var options = configuration
                                .GetSection("Identity")
                                .Get<IdentityOptions>()
                            ?? throw new NullReferenceException(nameof(QuartzOptions));

        services.AddDbContext<IdentityDbContext>((dbContextBuilder) =>
        {
            dbContextBuilder.UseNpgsql(options.ConnectionString, optionBuilder =>
            {
                optionBuilder.MigrationsHistoryTable(
                    tableName: HistoryRepository.DefaultTableName,
                    schema: IdentityDbContext.Schema);

                optionBuilder.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
            });
        });
    }
}