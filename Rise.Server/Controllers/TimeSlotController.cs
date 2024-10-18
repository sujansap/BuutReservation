using Microsoft.AspNetCore.Mvc;
using Rise.Shared.TimeSlots;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Rise.Domain.Timeslots;

namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController(ITimeSlotService timeSlotService, ILogger<TimeSlotController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly ITimeSlotService _timeSlotService = timeSlotService;


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
            TimeSlotRangeInfoDto timeSlotRangeInfoDto = await _timeSlotService.GetAllTimeSlotsInRange(
                startDate, endDate);
            _logger.LogDebug("Returning {days} days from {Start} to {End}", [timeSlotRangeInfoDto.TotalDays, timeSlotRangeInfoDto.Start, timeSlotRangeInfoDto.End]);

            return Ok(timeSlotRangeInfoDto);
        }


        /// <summary>
        /// Gets all time slots for the given date (+2 days from today onwards)
        /// </summary>
        /// <param name="year">The year</param>
        /// <param name="month">The month</param>
        /// <param name="day">The day</param>
        /// <returns>The timeslots for that day</returns>
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
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Date", [$"The given date is not valid"] }
                }));
            }

            var date = new DateTime(year, month, day);

            if (date <= DateTime.Today.AddDays(Reservation.MinDaysBetweenReservation))
            {
                return Ok(new List<TimeSlotDto>());
            }

            var timeSlots = await _timeSlotService.GetTimeSlotsByDate(year, month, day);

            return Ok(timeSlots);
        }

        private static bool IsValidDate(int year, int month, int day)
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

