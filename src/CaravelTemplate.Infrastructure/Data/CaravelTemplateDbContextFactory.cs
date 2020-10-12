using System;
using Caravel.AppContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaravelTemplate.Infrastructure.Data
{
    public class CaravelTemplateDbContextFactory : IDesignTimeDbContextFactory<CaravelTemplateDbContext>
    {
        public CaravelTemplateDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<CaravelTemplateDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString") ?? "Database=Temp"; 
           
            options.UseNpgsql(connectionString);
            
            return new CaravelTemplateDbContext(options.Options, new AppContextAccessor());
        }
    }
}