using CaravelTemplate.Identity;
using CaravelTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User = CaravelTemplate.Entities.User;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class EntityFrameworkExtension
    {
        public static void ConfigureEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConfig = configuration.GetSection("Database").Get<DatabaseSettings>();
            
            services.AddDbContext<CaravelTemplateTemplateDbContext>(options =>
            {
                options.UseNpgsql(dbConfig.ConnectionString);
            });

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<CaravelTemplateTemplateDbContext>();
        }
    }
}