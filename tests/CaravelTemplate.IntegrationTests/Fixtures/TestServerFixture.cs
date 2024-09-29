using System.Data.Common;
using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.PostgreSql;
using CaravelTemplate.IntegrationTests.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace CaravelTemplate.IntegrationTests.Fixtures;

public sealed class TestServerFixture : IAsyncLifetime
{
    public readonly TestingWebApplicationFactory TestServer;
    private readonly IServiceScope _serviceScope;
    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;

    public TestServerFixture()
    {
        TestServer = new TestingWebApplicationFactory();
        _serviceScope = TestServer.Services.CreateScope();
    }

    public ApplicationDbContext CreateApplicationDbContext()
    {
        return _serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public async Task CleanupApplicationDbContext()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await TestServer.InitializeAsync();

        var applicationDbContext = _serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var identityDbContext = _serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Ensure Database Migrations
        await applicationDbContext.Database.MigrateAsync();
        await identityDbContext.Database.MigrateAsync();

        // Configure Respawner to cleanup the database after each test
        _dbConnection = applicationDbContext.Database.GetDbConnection();
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await TestServer.DisposeAsync();
    }
}