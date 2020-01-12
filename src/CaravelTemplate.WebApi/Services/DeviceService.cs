using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Exceptions;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.WebApi.Exceptions;
using CaravelTemplate.WebApi.Models.Devices;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.WebApi.Services
{
    public class DeviceService
    {
        private readonly CaravelTemplateDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public DeviceService(CaravelTemplateDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<DeviceModel>> GetDevices(CancellationToken ctx)
        {
            var devices = await _dbContext.Devices.ToListAsync(ctx);

            return _mapper.Map<IEnumerable<DeviceModel>>(devices);
        }
        
        public async Task<DeviceModel> GetDeviceById(int id, CancellationToken ctx)
        {
            var device = await _dbContext.Devices.FindAsync(id);
            
            if (device == null)
            {
                throw new NotFoundException(ErrorCodes.DeviceNotFound);
            }
            
            return _mapper.Map<DeviceModel>(device);
        }
        
        public async Task<DeviceModel> CreateDevice(CreateDeviceModel deviceVm, CancellationToken ctx)
        {
            var device = _mapper.Map<Device>(deviceVm);
            
            _dbContext.Devices.Add(device);

            await _dbContext.SaveChangesAsync(ctx);

            return _mapper.Map<DeviceModel>(device);;
        }
        
        public async Task DeleteDevice(int id, CancellationToken ctx)
        {
            var device = await _dbContext.Devices.FindAsync(id);

            if (device == null)
            {
                throw new NotFoundException(ErrorCodes.DeviceNotFound);
            }

            _dbContext.Entry(device).State = EntityState.Deleted;

            await _dbContext.SaveChangesAsync(ctx);
        }
    }
}