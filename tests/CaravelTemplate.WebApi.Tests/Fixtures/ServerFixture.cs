using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caravel.Entities;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Identity;
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
        }

        public async Task SetupDatabase()
        {
            await DbContext.Database.MigrateAsync()!;
            await RoleSeeder.CreateRolesAsync(Server.Services.GetService<RoleManager<Role>>() ??
                                              throw new InvalidOperationException());
        }

        public async Task ClearDatabase()
        {
            DbContext.RemoveRange(DbContext.Books);
            DbContext.RemoveRange(DbContext.Events);
            DbContext.RemoveRange(DbContext.Users);
            DbContext.RemoveRange(DbContext.UserClaims);
            DbContext.RemoveRange(DbContext.UserLogins);
            DbContext.RemoveRange(DbContext.UserRoles);
            DbContext.RemoveRange(DbContext.UserTokens);
            DbContext.RemoveRange(DbContext.RefreshTokens);
            await DbContext?.SaveChangesAsync()!;
        }

        public async Task SeedDatabase(IEnumerable<Entity> entities)
        {
            DbContext?.AddRange(entities);
            await DbContext?.SaveChangesAsync()!;
        }

        public void Dispose()
        {
            DbContext?.Database.EnsureDeleted();
            Server.Dispose();
        }
    }
}