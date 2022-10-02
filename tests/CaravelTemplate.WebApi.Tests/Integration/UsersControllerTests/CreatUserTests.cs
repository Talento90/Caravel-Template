using System;
using System.Net;
using System.Threading.Tasks;
using Caravel.AspNetCore.Http;
using Caravel.Http;
using CaravelTemplate.Core.Users;
using CaravelTemplate.Core.Users.Commands;
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.UsersControllerTests
{
    [Collection("Integration")]
    public class CreateUserTests : IClassFixture<ServerFixture>, IDisposable
    {
        private const string ApiUrl = "/api/v1/users";
        private readonly ServerFixture _fixture;

        public CreateUserTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task Create_User_Created()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            var createUser = new CreateUserCommand()
            {
                Email = "test@caravel.com",
                Username = "caravel",
                Password = "!Caravel123",
                ConfirmPassword = "!Caravel123",
                FirstName = "Cara",
                LastName = "Vel"
            };

            // Act
            var response = await client.PostJsonAsync(ApiUrl, createUser);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var user = await response.Content.ReadAsJsonAsync<UserModel>();

            user.Email.Should().Be(createUser.Email);
        }
        
        [Fact]
        public async Task Create_User_Missing_Email_Bad_Request()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();
            var createUser = new CreateUserCommand()
            {
                Username = "caravel",
                Password = "caravel123",
                FirstName = "Cara",
                LastName = "Vel"
            };

            // Act
            var response = await client.PostJsonAsync(ApiUrl, createUser);

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