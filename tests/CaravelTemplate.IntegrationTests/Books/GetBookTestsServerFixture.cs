using System.Net;
using System.Net.Http.Json;
using CaravelTemplate.Application.Books;
using CaravelTemplate.Books;
using CaravelTemplate.IntegrationTests.Fixtures;

namespace CaravelTemplate.IntegrationTests.Books;

[Collection(nameof(IntegrationTestCollection))]
public class GetBookTestsServerFixture(TestServerFixture serverFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Should_Return_404StatusCode_When_Book_Does_Not_Exists()
    {
        var bookId = Guid.NewGuid();
        var httpClient = serverFixture.TestServer.CreateClient();

        var response = await httpClient.GetAsync($"/api/v1/books/{bookId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_Return_200StatusCode_When_Book_Exists()
    {
        var dbContext = serverFixture.CreateApplicationDbContext();

        var existingBook = new Book("Test", "Test");
        dbContext.Add(existingBook);
        await dbContext.SaveChangesAsync();

        var httpClient = serverFixture.TestServer.CreateClient();

        var response = await httpClient.GetAsync($"/api/v1/books/{existingBook.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var book = await response.Content.ReadFromJsonAsync<GetBookById.Response>();

        book.Should().NotBeNull();
        book!.Name.Should().Be(existingBook.Name);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await serverFixture.CleanupApplicationDbContext();
    }
}