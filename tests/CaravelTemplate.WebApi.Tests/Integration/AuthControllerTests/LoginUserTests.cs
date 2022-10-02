using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Caravel.AspNetCore.Http;
using Caravel.Http;
using CaravelTemplate.Core.Authentication;
using CaravelTemplate.Core.Authentication.Commands;
using CaravelTemplate.Core.Users;
using CaravelTemplate.WebApi.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration.AuthControllerTests
{
    [Collection("Integration")]
    public class LoginAuthTests : IClassFixture<ServerFixture>, IDisposable
    {
        private const string ApiUrl = "/api/v1/auth";
        private readonly ServerFixture _fixture;

        public LoginAuthTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Login_User_Success()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();

            var user = await FakeUser.CreateAsync(client);
            
            var loginUser = new LoginUserCommand()
            {
                Username = user.Username,
                Password = "!Caravel123"
            };

            // Act
            var response = await client.PostJsonAsync($"{ApiUrl}/login", loginUser);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = await response.Content.ReadAsJsonAsync<AccessTokenModel>();

            token.AccessToken.Should().NotBeEmpty();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            
            var responseGetUser = await client.GetAsync("/api/v1/users/profile");

            responseGetUser.StatusCode.Should().Be(HttpStatusCode.OK);
                
            var userModel = await responseGetUser.Content.ReadAsJsonAsync<UserModel>();
            
            user.Should().BeEquivalentTo(userModel);
        }
        
        [Fact]
        public async Task Login_User_With_Invalid_Access_Token_Should_Return_Unauthorized()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var responseGetUser = await client.GetAsync("/api/v1/users/profile");

            responseGetUser.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
                
            var error = await responseGetUser.Content.ReadAsJsonAsync<HttpError>();
            error.Code.Should().Be("invalid_token");
        }

        public void Dispose()
        {
            _fixture?.ClearDatabase();
        }
    }
}