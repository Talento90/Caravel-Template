using Caravel.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaravelTemplate.Identity.Data
{
    public class CaravelTemplateIdentityDbContextFactory : IDesignTimeDbContextFactory<CaravelTemplateIdentityDbContext>
    {
        public CaravelTemplateIdentityDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<CaravelTemplateIdentityDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("IdentityDatabase__ConnectionString") ?? "Database=Temp"; 
           
            options.UseNpgsql(connectionString);
            
            return new CaravelTemplateIdentityDbContext(options.Options, new AppContextAccessor());
        }
    }
}