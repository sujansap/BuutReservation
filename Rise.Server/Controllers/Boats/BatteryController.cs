using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Boats;

namespace Rise.Server.Controllers.Boats
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class BatteryController(IBatteryService batteryService, ILogger<BatteryController> logger) : ControllerBase
    {

        private readonly ILogger<BatteryController> _logger = logger;
        private readonly IBatteryService _batteryService = batteryService;

        // TODO document
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newBattery"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatteryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutBattery(
            [Range(0, int.MaxValue, ErrorMessage = "Battery id must be positive")]
            int id,
            [FromBody]
            BatteryUpdateDto newBattery)
        {
            _logger.LogInformation("PUT battery: {id}", id);
            _logger.LogDebug("PUT battery: {id} with information {newBattery}", id, newBattery);
            BatteryDto battery = await _batteryService.UpdateBattery(id, newBattery);
            _logger.LogInformation("PUT done updating battery: {id}", id);
            _logger.LogDebug("PUT done updating battery: {id}. New information {battery}", id, battery);
            return Ok(battery);
        }
    }
}
