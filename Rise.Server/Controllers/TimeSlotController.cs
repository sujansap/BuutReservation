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


        /// <summary>
        /// Gets all time slots during the given date range
        /// </summary>
        /// <param name="startDay">Date from where the range starts</param>
        /// <param name="endDay">Date from where the range ends (inclusive)</param>        
        /// <returns></returns>
        [HttpGet("range")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeSlotRangeInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableTimeSlotsInMonth(
            [FromQuery]
            DateOnly startDay,
            [FromQuery]
            DateOnly endDay
            )
        {
            _logger.LogInformation("GET range?startDay={startDay}&endDay={endDay}", [startDay, endDay]);
            _logger.LogDebug("Checking if {startDay} becomes before {endDay} ", [startDay, endDay]);
            if (startDay > endDay)
            {
                _logger.LogWarning("Invalid date range: {startDay} comes after {endDay}", [startDay, endDay]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "DateRange", [$"The start date ({startDay}) cannot be later than the end date ({endDay})"] }
                }));
            }

            _logger.LogDebug("Getting days between range {startDay} and {endDay} from service layer", [startDay, endDay]);
            TimeSlotRangeInfoDto timeSlotRangeInfoDto = await timeSlotService.GetAllTimeSlotsInRange(
                startDay, endDay);
            _logger.LogDebug("Returning {days} days from {Start} to {End}", [timeSlotRangeInfoDto.TotalDays, timeSlotRangeInfoDto.Start, timeSlotRangeInfoDto.End]);

            return Ok(timeSlotRangeInfoDto);
        }
    }
}

