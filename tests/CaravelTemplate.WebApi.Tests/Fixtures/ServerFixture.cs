using System;
using System.Linq;
using CaravelTemplate.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Tests.Fixtures
{
    public sealed class ServerFixture : IDisposable
    {
        public readonly TestServer Server;
        public CaravelTemplateDbContext DbContext => Server.Services.GetService<CaravelTemplateDbContext>();

        public ServerFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new WebHostBuilder()
                .UseEnvironment("local")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
                
            
            Server = new TestServer(builder);
        }
        
        public void SeedDatabase(params object[] entities)
        {
            DbContext.AddRange(entities);
            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            Server.Dispose();
        }
    }
}