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
    public class GetBookTests : IClassFixture<ServerFixture>, IDisposable
    {
        private const string ApiUrl = "/api/v1/books";
        private readonly ServerFixture _fixture;
        
        public GetBookTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task Get_Book_Ok()
        {
            // Arrange
            await _fixture.SetupDatabase();
            var client = _fixture.Server.CreateClient();
            var books = FakeData.BookFaker().Generate(1).ToArray();
            await _fixture.SeedDatabase(books);
            
            var book = books.First();

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
            await _fixture.SetupDatabase();
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
            _fixture?.ClearDatabase();
        }
    }
}