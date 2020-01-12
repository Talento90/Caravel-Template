using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.WebApi.Models.Devices;
using CaravelTemplate.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaravelTemplate.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly DeviceService _deviceService;
        
        public DevicesController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceModel>>> Get(CancellationToken ctx)
        {
            var devices = await _deviceService.GetDevices(ctx);
            
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceModel>> GetById(int id, CancellationToken ctx)
        {
            var device = await _deviceService.GetDeviceById(id, ctx);

            return Ok(device);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateDeviceModel deviceVm, CancellationToken ctx)
        {
            var model = await _deviceService.CreateDevice(deviceVm, ctx);

            return CreatedAtAction(nameof(GetById), new {id = model.Id}, model);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ctx)
        {
            await _deviceService.DeleteDevice(id, ctx);

            return NoContent();
        }
        
    }
}