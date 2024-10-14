using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.TimeSlots;

namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController(ITimeSlotService timeSlotService, ILogger<TimeSlotController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeSlotRangeInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TimeSlotRangeInfoDto> Get(
            [FromRoute]
            [Range(1, 9999, ErrorMessage = "Year cannot be negative")]
            int year,
            [FromRoute]
            [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
            int month,
            [FromQuery]
            bool includeCrossOverDays = false
            )
        {
            _logger.LogInformation("GET TimeSlot/{year}/{month}?includeCrossOverDays={includeCrossOverDays}", [year, month, includeCrossOverDays]);
            _logger.LogDebug("Getting days from year {year} and mont {month} including cross over days = {includeCrossOverDays} from service layer", [year, month, includeCrossOverDays]);
            TimeSlotRangeInfoDto timeSlotRangeInfoDto = await timeSlotService.GetAllTimeSlotsFromMonth(
                year,
                month,
                includeCrossOverDays);
            _logger.LogDebug("Returning {days} days from {Start} to {End}", [timeSlotRangeInfoDto.TotalDays, timeSlotRangeInfoDto.Start, timeSlotRangeInfoDto.End]);

            return timeSlotRangeInfoDto;
        }
    }
}

