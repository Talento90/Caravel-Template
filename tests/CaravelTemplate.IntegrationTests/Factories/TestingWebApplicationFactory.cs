using System.Data.Common;
using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.PostgreSql;
using DotNet.Testcontainers.Builders;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace CaravelTemplate.IntegrationTests.Factories;

public sealed class TestingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("caravel")
        .WithUsername("user")
        .WithPassword("password123")
        .WithPortBinding(5432, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management-alpine")
        .WithUsername("guest")
        .WithPassword("guest")
        .WithPortBinding(5672, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
        .Build();
    
    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;
    private IServiceScope _serviceScope = null!;

    public ApplicationDbContext CreateApplicationDbContext()
    {
        return _serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public async Task CleanupApplicationDbContext()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // builder.ConfigureAppConfiguration((_, configBuilder) =>
        // {
        //     configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        //     {
        //         { "FeatureManagement:FeatureBook", "false" }
        //     });
        // });
        
        builder.ConfigureTestServices(services =>
        {
            // // Configure MassTransit Testing
            services.AddMassTransitTestHarness((busConfig =>
            {
                //busConfig.AddConsumer<BookCreatedConsumer>();
                busConfig.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host(_rabbitMqContainer.GetConnectionString());
                });
            }));

            //Override services for tests
            SetupTestDbContext(services);
        });
    }

    private void SetupTestDbContext(IServiceCollection services)
    {
        RemoveDescriptor(services, typeof(DbContextOptions<ApplicationDbContext>));
        RemoveDescriptor(services, typeof(DbContextOptions<IdentityDbContext>));

        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(_postgreSqlContainer.GetConnectionString());
        });
        services.AddDbContext<IdentityDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(_postgreSqlContainer.GetConnectionString());
        });
    }

    private static void RemoveDescriptor(IServiceCollection services, Type serviceType)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == serviceType);

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgreSqlContainer.StartAsync(), _rabbitMqContainer.StartAsync());
        
        _serviceScope = Services.CreateScope();
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

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(_postgreSqlContainer.StopAsync(), _rabbitMqContainer.StopAsync());
    }
}