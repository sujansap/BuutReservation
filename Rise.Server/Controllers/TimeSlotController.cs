using Microsoft.AspNetCore.Mvc;
using Rise.Services.TimeSlots;
using Rise.Shared.TimeSlots;

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

        // GET api/timeslot?date=2024-10-08
        [HttpGet]
        public async Task<IActionResult> GetTimeSlotsByDate([FromQuery] DateTime date)
        {
            var timeSlots = await _timeSlotService.GetTimeSlotsByDate(date);

            if (timeSlots == null || !timeSlots.Any())
            {
                return NotFound("No TimeSlots found for the given date.");
            }

            return Ok(timeSlots);
        }
    }
}

