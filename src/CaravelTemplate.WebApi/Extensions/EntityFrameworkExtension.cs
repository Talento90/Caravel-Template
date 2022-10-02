using CaravelTemplate.Identity;
using CaravelTemplate.Identity.Data;
using CaravelTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class EntityFrameworkExtension
    {
        public static void ConfigureEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CaravelTemplateTemplateDbContext>(options =>
            {
                var dbConfig = configuration.GetSection("Database").Get<DatabaseSettings>();
                options.UseNpgsql(dbConfig.ConnectionString, npqOptions =>
                {
                    npqOptions.EnableRetryOnFailure();
                });
            });

            services.AddDbContext<CaravelTemplateIdentityDbContext>(options =>
                {
                    var dbConfig = configuration.GetSection("IdentityDatabase").Get<DatabaseSettings>();
                    options.UseNpgsql(dbConfig.ConnectionString, npqOptions =>
                    {
                        npqOptions.EnableRetryOnFailure();
                    });
                })
                .AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<CaravelTemplateIdentityDbContext>();
        }
    }
}