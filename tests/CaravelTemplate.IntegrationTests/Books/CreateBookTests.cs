using System.Net;
using System.Net.Http.Json;
using Caravel.AspNetCore.Http;
using CaravelTemplate.Adapter.MassTransit.Consumers;
using CaravelTemplate.Application.Books;
using CaravelTemplate.Application.Messaging;
using CaravelTemplate.IntegrationTests.Factories;
using MassTransit.Testing;

namespace CaravelTemplate.IntegrationTests.Books;

// Shared Fixture across all Tests
[Collection(nameof(IntegrationTestCollection))]
public class CreateBookTests(TestingWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_Return_400StatusCode_When_Book_Not_Valid()
    {
        var bookRequest = new CreateBook.Request("", "My Testing Book");
        var httpClient = TestServer.CreateClient();

        var response = await httpClient.PostAsJsonAsync($"/api/v1/books", bookRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var apiError = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();

        apiError.Should().NotBeNull();
        apiError!.Code.Should().Be("validation_payload");
        apiError!.Errors.Should().HaveCount(1);
        apiError!.Errors!.First().Key.Should().Be("Name");
    }

    [Fact]
    public async Task Should_Return_201StatusCode_When_Book_Is_Valid()
    {
        var bookRequest = new CreateBook.Request("My Book", "My Testing Book");
        var httpClient = TestServer.CreateClient();

        var response = await httpClient.PostAsJsonAsync($"/api/v1/books", bookRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var bookResponse = await response.Content.ReadFromJsonAsync<CreateBook.Response>();

        bookResponse.Should().NotBeNull();
        bookResponse!.Name.Should().Be(bookRequest.Name);
        bookResponse!.Description.Should().Be(bookRequest.Description);

        var harness = TestServer.Services.GetTestHarness();
        var consumerTestHarness = harness.GetConsumerHarness<BookCreatedConsumer>();

        var consumer =
            await consumerTestHarness.Consumed.Any<BookCreatedMessage>(x => x.Context.Message.Book.Name == "My Book");

        consumer.Should().BeTrue();
    }
}