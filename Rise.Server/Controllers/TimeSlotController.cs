using Microsoft.AspNetCore.Mvc;
using Rise.Shared.TimeSlots;

namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController(ITimeSlotService timeSlotService) : ControllerBase
    {
        private readonly ITimeSlotService timeSlotService = timeSlotService;

        // GET api/timeslot?date=2024-10-08
        [HttpGet]
        public async Task<IActionResult> GetTimeSlotsByDate([FromQuery] DateTime date)
        {
            var timeSlots = await timeSlotService.GetTimeSlotsByDate(date);

            if (timeSlots == null || !timeSlots.Any())
            {
                return NotFound("No TimeSlots found for the given date.");
            }

            return Ok(timeSlots);
        }

        /// <summary>
        /// Gets all time slots during the given month of a year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="includeCrossOverDays">If days need to be included from the weeks where in the month crosses over from/into the other</param>
        /// <returns></returns>
        [HttpGet("{year}/{month}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TimeSlotRangeInfoDto> Get(
            int year,
            int month,
            bool includeCrossOverDays = false
            )
        {
            // TODO add validation
            // TODO add simple data range
            TimeSlotRangeInfoDto timeSlotRangeInfoDto = await timeSlotService.GetAllTimeSlotsFromMonth(
                year,
                month,
                includeCrossOverDays);
            return timeSlotRangeInfoDto;
        }
    }
}

