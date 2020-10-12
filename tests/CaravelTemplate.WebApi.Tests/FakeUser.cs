using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Caravel.Http;
using CaravelTemplate.Core.Authentication;
using CaravelTemplate.Core.Authentication.Commands;
using CaravelTemplate.Core.Users;
using CaravelTemplate.Core.Users.Commands;
using FluentAssertions;

namespace CaravelTemplate.WebApi.Tests
{
    public static class FakeUser
    {
        public static async Task<UserModel> CreateAsync(HttpClient client)
        {
            var username = Guid.NewGuid();
            
            var createUser = new CreateUserCommand()
            {
                Email = $"{username}@caravel.com",
                Username = username.ToString(),
                Password = "!Caravel123",
                ConfirmPassword = "!Caravel123",
                FirstName = "Cara",
                LastName = "Vel"
            };
            
            var response = await client.PostJsonAsync("api/v1/users", createUser);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            return await response.Content.ReadAsJsonAsync<UserModel>(); ;
        }
        
        public static async Task<AccessTokenModel> LoginAsync(HttpClient client, string username)
        {
            var login = new LoginUserCommand()
            {
                Username = username,
                Password = "!Caravel123",
            };
            
            var response = await client.PostJsonAsync("api/v1/auth", login);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            return await response.Content.ReadAsJsonAsync<AccessTokenModel>(); ;
        }
    }
}