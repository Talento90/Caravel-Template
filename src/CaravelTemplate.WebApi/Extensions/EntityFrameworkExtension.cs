using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class EntityFrameworkExtension
    {
        public static void ConfigureEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConfig = configuration.GetSection("Database").Get<DatabaseSettings>();

            services.AddDbContext<CaravelTemplateDbContext>(options =>
            {
                if (dbConfig.IsInMemory)
                {
                    options.UseInMemoryDatabase("CaravelTemplateDbContext");
                }
                else
                {
                    options.UseNpgsql(dbConfig.ConnectionString);
                }
            });

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<CaravelTemplateDbContext>();
        }
    }
}