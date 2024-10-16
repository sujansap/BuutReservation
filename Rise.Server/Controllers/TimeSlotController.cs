using Microsoft.AspNetCore.Mvc;
using Rise.Services.TimeSlots;
using Rise.Shared.TimeSlots;
using System.ComponentModel.DataAnnotations;

namespace Rise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }


        [HttpGet("{year}/{month}/{day}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeSlotDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTimeSlotsByDate(
            [FromRoute]
            [Range(1, 9999, ErrorMessage = "Year must be between 1 and 9999")]
            int year,
            [FromRoute]
            [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
            int month,
            [FromRoute]
            [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
            int day)
        {
            // Validate the date parameters
            if (!IsValidDate(year, month, day))
            {
                return BadRequest("Invalid date parameters.");
            }

            var timeSlots = await _timeSlotService.GetTimeSlotsByDate(year, month, day);

            return Ok(timeSlots);
        }

        private bool IsValidDate(int year, int month, int day)
        {
            try
            {
                var date = new DateTime(year, month, day);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
    }
}

