using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Caravel.AspNetCore.Http;
using Caravel.Exceptions;
using CaravelTemplate.Core.Books;
using CaravelTemplate.Core.Books.Commands;
using CaravelTemplate.Entities;
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.BooksControllerTests
{
    public class GetBookTests : IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;
        private readonly Book[] _books;
        
        public GetBookTests()
        {
            _books = FakeData.BookFaker.Generate(1).ToArray();
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

            bookModel.Should().BeEquivalentTo(book);
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

            error.Code.Should().Be(ErrorCodes.BookNotFound.Code);
            error.Title.Should().Be(ErrorCodes.BookNotFound.Message);
        }

        public void Dispose()
        {
            _fixture?.Dispose();
        }
    }
}