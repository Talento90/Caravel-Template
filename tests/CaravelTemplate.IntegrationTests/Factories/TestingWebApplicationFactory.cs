using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace CaravelTemplate.IntegrationTests.Factories;

public sealed class TestingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("caravel")
        .WithUsername("user")
        .WithPassword("password123")
        .Build();


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
            //Override services for tests
            SetupTestDbContext(services);
        });
        
        base.ConfigureWebHost(builder);
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
        await _postgreSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}