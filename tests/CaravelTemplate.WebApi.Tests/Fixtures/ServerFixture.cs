using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caravel.Entities;
using CaravelTemplate.Identity;
using CaravelTemplate.Identity.Data;
using CaravelTemplate.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Tests.Fixtures
{
    public sealed class ServerFixture : IDisposable
    {
        public readonly TestServer Server;

        public CaravelTemplateTemplateDbContext DbContext =>
            Server.Services.GetService<CaravelTemplateTemplateDbContext>()
            ?? throw new InvalidOperationException();

        public CaravelTemplateIdentityDbContext IdentityDbContext =>
            Server.Services.GetService<CaravelTemplateIdentityDbContext>()
            ?? throw new InvalidOperationException();

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

            IdentityDbContext.Database.Migrate();
            DbContext.Database.Migrate();
        }

        public async Task SetupDatabase()
        {
            using var scope = Server.Services.CreateScope();
            await RoleSeeder.CreateRolesAsync(scope.ServiceProvider.GetService<RoleManager<Role>>()!);
        }

        public async Task ClearDatabase()
        {
            DbContext.RemoveRange(DbContext.Books);
            DbContext.RemoveRange(DbContext.Events);
            await DbContext.SaveChangesAsync()!;

            IdentityDbContext.RemoveRange(IdentityDbContext.Users);
            IdentityDbContext.RemoveRange(IdentityDbContext.UserClaims);
            IdentityDbContext.RemoveRange(IdentityDbContext.UserLogins);
            IdentityDbContext.RemoveRange(IdentityDbContext.UserRoles);
            IdentityDbContext.RemoveRange(IdentityDbContext.UserTokens);
            IdentityDbContext.RemoveRange(IdentityDbContext.RefreshTokens);
            await IdentityDbContext.SaveChangesAsync();
        }

        public async Task SeedDatabase(IEnumerable<Entity> entities)
        {
            DbContext.AddRange(entities);
            await DbContext.SaveChangesAsync()!;
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            IdentityDbContext.Database.EnsureDeleted();
            Server.Dispose();
        }
    }
}