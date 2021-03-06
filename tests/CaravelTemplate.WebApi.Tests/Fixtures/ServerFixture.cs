using System;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Tests.Fixtures
{
    public sealed class ServerFixture : IDisposable
    {
        public readonly TestServer Server;
        public CaravelTemplateTemplateDbContext? DbContext => Server.Services.GetService<CaravelTemplateTemplateDbContext>();

        public ServerFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            Server = new TestServer(builder);
            
            configuration["Jwt:Audience"] = Server.BaseAddress.ToString();
            
            RoleSeeder.CreateRolesAsync(Server.Services.GetService<RoleManager<Role>>() ?? 
                                        throw new InvalidOperationException());
        }

        public void SeedDatabase(params object[] entities)
        {
            DbContext?.AddRange(entities);
            DbContext?.SaveChanges();
        }

        public void Dispose()
        {
            DbContext?.Database.EnsureDeleted();
            Server.Dispose();
        }
    }
}