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
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.BooksControllerTests
{
    [Collection("Integration")]
    public class UpdateBookTests : IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;
        private readonly Book[] _books;

        public UpdateBookTests()
        {
            _books = FakeData.BookFaker().Generate(1).ToArray();
            _fixture = new ServerFixture();
            _fixture.SeedDatabase(_books);
        }
        
        [Fact]
        public async Task Update_Book_Created()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            var book = _books.First();
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
            var client = _fixture.Server.CreateClient();
            var updateBook = new UpdateBookCommand
            {
                Name = "New Caravel Book",
                Description = "Book about Caravel package"
            };

            // Act
            var response = await client.PutJsonAsync($"{ApiUrl}/{Guid.NewGuid()}", updateBook);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var error = await response.Content.ReadAsJsonAsync<HttpError>();

            error.Code.Should().Be(Errors.BookNotFound);
        }
        
        [Fact]
        public async Task Update_Book_Bad_Request()
        {
            // Arrange
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

            error.Code.Should().Be(Errors.InvalidFields);
        }

        public void Dispose()
        {
            _fixture?.Dispose();
        }
    }
}