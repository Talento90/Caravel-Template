using System;
using System.Net;
using System.Threading.Tasks;
using CaravelTemplate.WebApi.Tests.Fixtures;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Integration
{
    public class DevicesControllerTests : IDisposable
    {
        private readonly ServerFixture _fixture;

        public DevicesControllerTests()
        {
            _fixture = new ServerFixture();
        }
        
        [Fact]
        public async Task Get_AllDevices_Success()
        {
            // Arrange
            var client = _fixture.Server.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/devices");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _fixture?.Dispose();
        }
    }
}