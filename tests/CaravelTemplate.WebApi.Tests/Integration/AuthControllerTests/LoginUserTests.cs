using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
    public class LoginAuthTests : IDisposable
    {
        private const string ApiUrl = "/api/v1/auth";
        private readonly ServerFixture _fixture;

        public LoginAuthTests()
        {
            _fixture = new ServerFixture();
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

        public void Dispose()
        {
            _fixture?.Dispose();
        }
    }
}