using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.Adapter.PostgreSql;

public static class PostgreSqlExtensions
{
    public static void RegisterPostgreSql(this IServiceCollection services, PostgreSqlOptions options)
    {
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(options.ConnectionString);
        });
    }
}