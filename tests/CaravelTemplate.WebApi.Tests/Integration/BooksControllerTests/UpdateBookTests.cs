using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using Caravel.AspNetCore.Http;
using Caravel.Exceptions;
using Caravel.Http;
using CaravelTemplate.Core;
using CaravelTemplate.Core.Books;
using CaravelTemplate.Core.Books.Commands;
using CaravelTemplate.Entities;
using CaravelTemplate.Errors;
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.BooksControllerTests
{
    [Collection("Integration")]
    public class UpdateBookTests : IClassFixture<ServerFixture>, IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;

        public UpdateBookTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task Update_Book_Created()
        {
            // Arrange
            await _fixture.SetupDatabase();
            var client = _fixture.Server.CreateClient();
            var books = FakeData.BookFaker().Generate(1).ToArray();
            await _fixture.SeedDatabase(books);
            
            var book = books.First();
            var updateBook = new UpdateBookCommand
            {
                Name = "New Caravel Book",
                Description = "Book about Caravel package"
            };

            // Act
            var response = await client.PutJsonAsync($"{ApiUrl}/{book.Id}", updateBook);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var bookModel = await response.Content.ReadAsJsonAsync<BookModel>();

            bookModel.Name.Should().Be(updateBook.Name);
            bookModel.Description.Should().Be(updateBook.Description);
        }
        
        [Fact]
        public async Task Update_Book_Not_Found()
        {
            // Arrange
            await _fixture.SetupDatabase();
            var client = _fixture.Server.CreateClient();
            var bookId = Guid.NewGuid();
            var updateBook = new UpdateBookCommand
            {
                Name = "New Caravel Book",
                Description = "Book about Caravel package"
            };

            // Act
            var response = await client.PutJsonAsync($"{ApiUrl}/{bookId}", updateBook);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var error = await response.Content.ReadAsJsonAsync<HttpError>();

            error.Code.Should().Be(BookErrors.NotFound(bookId).Code);
        }
        
        [Fact]
        public async Task Update_Book_Bad_Request()
        {
            // Arrange
            await _fixture.SetupDatabase();
            var client = _fixture.Server.CreateClient();
            var updateBook = new UpdateBookCommand
            {
                Name = new Faker().Random.String2(100),
                Description = "Book about Caravel package"
            };

            // Act
            var response = await client.PutJsonAsync($"{ApiUrl}/{Guid.NewGuid()}", updateBook);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var error = await response.Content.ReadAsJsonAsync<HttpError>();

            error.Code.Should().Be("invalid_fields");
        }

        public void Dispose()
        {
            _fixture?.ClearDatabase();
        }
    }
}