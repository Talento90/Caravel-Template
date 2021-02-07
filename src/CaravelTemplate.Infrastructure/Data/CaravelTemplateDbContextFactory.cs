using System;
using Caravel.AppContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaravelTemplate.Infrastructure.Data
{
    public class CaravelTemplateDbContextFactory : IDesignTimeDbContextFactory<CaravelTemplateTemplateDbContext>
    {
        public CaravelTemplateTemplateDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<CaravelTemplateTemplateDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString") ?? "Database=Temp"; 
           
            options.UseNpgsql(connectionString);
            
            return new CaravelTemplateTemplateDbContext(options.Options, new AppContextAccessor());
        }
    }
}