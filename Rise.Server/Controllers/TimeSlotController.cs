using Microsoft.AspNetCore.Mvc;
using Rise.Shared.TimeSlots;
using Swashbuckle.AspNetCore.Annotations;

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
        /// <param name="startDate">Date from where the range starts</param>
        /// <param name="endDate">Date from where the range ends (inclusive)</param>        
        /// <returns></returns>
        [HttpGet("range")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeSlotRangeInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableTimeSlotsInMonth(
            [FromQuery, SwaggerParameter(Required = true)]
            DateOnly startDate,
            [FromQuery, SwaggerParameter(Required = true)]
            DateOnly endDate
            )
        {
            _logger.LogInformation("GET range?startDate={startDate}&endDay={endDay}", [startDate, endDate]);
            _logger.LogDebug("Checking if {startDate} becomes before {endDay} ", [startDate, endDate]);
            if (startDate > endDate)
            {
                _logger.LogWarning("Invalid date range: {startDate} comes after {endDay}", [startDate, endDate]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "DateRange", [$"The start date ({startDate}) cannot be later than the end date ({endDate})"] }
                }));
            }

            _logger.LogDebug("Getting days between range {startDate} and {endDay} from service layer", [startDate, endDate]);
            TimeSlotRangeInfoDto timeSlotRangeInfoDto = await timeSlotService.GetAllTimeSlotsInRange(
                startDate, endDate);
            _logger.LogDebug("Returning {days} days from {Start} to {End}", [timeSlotRangeInfoDto.TotalDays, timeSlotRangeInfoDto.Start, timeSlotRangeInfoDto.End]);

            return Ok(timeSlotRangeInfoDto);
        }
    }
}

