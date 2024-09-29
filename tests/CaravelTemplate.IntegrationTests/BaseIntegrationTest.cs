using CaravelTemplate.IntegrationTests.Factories;

namespace CaravelTemplate.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly TestingWebApplicationFactory TestServer;

    protected BaseIntegrationTest(TestingWebApplicationFactory testingWebApplicationFactory)
    {
        TestServer = testingWebApplicationFactory;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await TestServer.CleanupApplicationDbContext();
    }
}