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
using CaravelTemplate.Errors;
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.BooksControllerTests
{
    [Collection("Integration")]
    public class DeleteBookTests : IClassFixture<ServerFixture>, IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;

        public DeleteBookTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task Delete_Book_No_Content()
        {
            // Arrange
            await _fixture.SetupDatabase();
            var client = _fixture.Server.CreateClient();
            
            var books = FakeData.BookFaker().Generate(1).ToArray();
            await _fixture.SeedDatabase(books);
            
            var book = books.First();

            // Act
            var response = await client.DeleteAsync($"{ApiUrl}/{book.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _fixture.DbContext?.Books.Any().Should().BeFalse();
        }
        
        [Fact]
        public async Task Delete_Book_Not_Found()
        {
            // Arrange
            await _fixture.SetupDatabase();
            var client = _fixture.Server.CreateClient();
            var bookId = Guid.NewGuid();
            // Act
            var response = await client.DeleteAsync($"{ApiUrl}/{bookId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var error = await response.Content.ReadAsJsonAsync<HttpError>();

            error.Code.Should().Be(BookErrors.NotFound(bookId).Code);
        }

        public void Dispose()
        {
            _fixture?.ClearDatabase();
        }
    }
}