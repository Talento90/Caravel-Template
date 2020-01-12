using AutoMapper;
using CaravelTemplate.WebApi.Models.Devices;

namespace CaravelTemplate.WebApi.Infrastructure.Mapper
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<CreateDeviceModel, Device>(); 
            CreateMap<Device, DeviceModel>();
        }
    }
}