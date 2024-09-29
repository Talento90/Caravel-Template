using CaravelTemplate.IntegrationTests.Factories;

namespace CaravelTemplate.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<TestingWebApplicationFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    // Reference: https://xunit.net/docs/shared-context
}