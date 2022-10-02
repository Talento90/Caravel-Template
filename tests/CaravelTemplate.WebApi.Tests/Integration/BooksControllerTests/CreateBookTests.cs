using System;
using System.Net;
using System.Threading.Tasks;
using Caravel.AspNetCore.Http;
using Caravel.Exceptions;
using Caravel.Http;
using CaravelTemplate.Core.Books;
using CaravelTemplate.Core.Books.Commands;
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.BooksControllerTests
{
    [Collection("Integration")]
    public class CreateBookTests : IClassFixture<ServerFixture>, IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;

        public CreateBookTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task Create_Book_Created()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            var createBook = new CreateBookCommand
            {
                Name = "Caravel Book",
                Description = "Book about Caravel package"
            };

            // Act
            var response = await client.PostJsonAsync(ApiUrl, createBook);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var book = await response.Content.ReadAsJsonAsync<BookModel>();

            book.Name.Should().Be(createBook.Name);
            book.Description.Should().Be(createBook.Description);
        }
        
        [Fact]
        public async Task Create_Book_Missing_Name_Bad_Request()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            var createBook = new CreateBookCommand
            {
                Description = "Book without a name"
            };

            // Act
            var response = await client.PostJsonAsync(ApiUrl, createBook);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var error = await response.Content.ReadAsJsonAsync<HttpError>();

            error.Code.Should().Be("invalid_fields");
            error.Title.Should().Be("Payload contains invalid fields.");
        }

        public void Dispose()
        {
            _fixture?.ClearDatabase();
        }
    }
}