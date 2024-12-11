using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Boats;

namespace Rise.Server.Controllers.Boats
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class BoatController(IBoatService boatService, ILogger<BoatController> logger) : ControllerBase
    {
        private readonly ILogger<BoatController> _logger = logger;
        private readonly IBoatService _boatService = boatService;

        /// <summary>
        /// Gets the count of active boats
        /// </summary>
        /// <returns>The number of non-deleted boats</returns>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveBoatsCount()
        {
            _logger.LogInformation("GET boats count");
            var count = await _boatService.GetActiveBoatsCountAsync();
            _logger.LogInformation("GET done fetching boats count: {count}", count);
            return Ok(count);
        }


    }
}