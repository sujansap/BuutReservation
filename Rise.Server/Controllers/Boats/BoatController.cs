using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared;
using Rise.Shared.Boats;

namespace Rise.Server
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class BoatController : ControllerBase
    {
        private readonly ILogger<BoatController> _logger;
        private readonly IBoatService _boatService;


        public BoatController(IBoatService boatService, ILogger<BoatController> logger)
        {
            _logger = logger;
            _boatService = boatService;
        }

        /// <summary>
        /// Haalt alle boten op.
        /// </summary>
        /// <returns>Een lijst van boten.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBoats()
        {
            try
            {
                var boats = await _boatService.GetAllBoatsAsync();
                return Ok(boats);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
            }


        }
    }
}
