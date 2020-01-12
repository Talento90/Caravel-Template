using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CaravelTemplate.WebApi.Tests.Fixtures
{
    public sealed class ServerFixture : IDisposable
    {
        public readonly TestServer Server;
        
        public ServerFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddInMemoryCollection()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new WebHostBuilder()
                .UseEnvironment("local")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
            
            Server = new TestServer(builder);
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }

    public static class TestServerExtensions
    {
        private static string GenerateJwtToken(string userId)
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim("username", userId));

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity
            };

            return new JwtSecurityTokenHandler().CreateEncodedJwt(securityTokenDescriptor);
        }

        public static HttpClient CreateAuthenticatedClient(this TestServer server, string? userId = null)
        {
            var client = server.CreateClient();

            var token = GenerateJwtToken(userId ?? Guid.NewGuid().ToString());

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            return client;
        }
    }
}