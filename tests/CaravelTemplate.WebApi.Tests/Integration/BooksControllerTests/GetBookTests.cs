using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
using Xunit.Sdk;

namespace CaravelTemplate.WebApi.Tests.Integration.BooksControllerTests
{
    [Collection("Integration")]
    public class GetBookTests : IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;
        private readonly Book[] _books;
        
        public GetBookTests()
        {
            _books = FakeData.BookFaker().Generate(1).ToArray();
            _fixture = new ServerFixture();
            _fixture.SeedDatabase(_books);
        }
        
        [Fact]
        public async Task Get_Book_Ok()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            var book = _books.First();

            // Act
            var response = await client.GetAsync($"{ApiUrl}/{book.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var bookModel = await response.Content.ReadAsJsonAsync<BookModel>();

            bookModel.Id.Should().Be(book.Id);
            bookModel.Name.Should().Be(book.Name);
            bookModel.Description.Should().Be(book.Description);
        }
        
        [Fact]
        public async Task Get_Book_Not_Found()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            
            // Act
            var response = await client.GetAsync($"{ApiUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var error = await response.Content.ReadAsJsonAsync<HttpError>();

            error.Code.Should().Be(Errors.BookNotFound);
        }

        public void Dispose()
        {
            _fixture?.Dispose();
        }
    }
}