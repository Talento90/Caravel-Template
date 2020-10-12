using System;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            Server = new TestServer(builder);
            
            configuration["Jwt:Audience"] = Server.BaseAddress.ToString();
            
            RoleSeeder.CreateRolesAsync(Server.Services.GetService<RoleManager<Role>>());
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