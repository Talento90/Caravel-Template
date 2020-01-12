using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.WebApi.Entities;
using CaravelTemplate.WebApi.Infrastructure.Data;
using CaravelTemplate.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CaravelTemplate.WebApi.Tests.Unit.Services
{
    public class DevicesServiceTests : IDisposable
    {
        private readonly CaravelTemplateDbContext _dbContext;
        private readonly DeviceService _deviceService;

        public DevicesServiceTests()
        {
            var options = new DbContextOptionsBuilder<CaravelTemplateDbContext>()
                .UseInMemoryDatabase(databaseName: "CaravelTemplateDb")
                .Options;
            
            var config = new MapperConfiguration(cfg => {
                cfg.AddMaps(typeof(Startup).Assembly);
            });

            _dbContext = new CaravelTemplateDbContext(options);
            _deviceService = new DeviceService(_dbContext, config.CreateMapper());
        }
        
        [Fact]
        public async Task Get_AllDevices_Success()
        {
            // Arrange
            _dbContext.Devices.Add(new Device {Id = 1});
            _dbContext.Devices.Add(new Device {Id = 2});

            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            var devices = await _deviceService.GetDevices(CancellationToken.None);

            // Assert
            Assert.Equal(2, devices.Count());
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}