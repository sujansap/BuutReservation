using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Server.Common.Filters;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using FluentValidation;
namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
        {
            _logger = logger;
            _reservationService = reservationService;
        }

        /// <summary>
        /// Gets all reservations for a user.
        /// </summary>
        /// <remarks>
        /// <para>To get the first page, set cursor and isNextPage to null. For ther other pages, use the following logic:</para> 
        /// <para>How it works, set the cursor to an Id you get from a request with the following valid parameters (not null):
        /// <br/>
        /// <para>NextId and isNextPage equals true: get the next page </para>
        /// <br/>
        /// <para>PreviousId and isNextPage equals false: get the previous page</para></para>
        /// </remarks>
        /// 
        /// <param name="cursor">The Id of a entity to fetch relative to, i.e. cursor can be the Id of NextId or PreviousId (see above).</param>
        /// <param name="isNextPage">If avaiable, true to get next page or false: to get last page.</param>   
        /// <param name="getPast">Get all reservations in the past.</param>
        /// <param name="pageSize">Number of items to get in a page.</param>
        /// <returns>List of reservations</returns>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemsPageDto<ReservationDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentUserReservations(
            [FromQuery] int? cursor,
            [FromQuery] bool? isNextPage,
            [FromQuery] bool getPast = false,
            [FromQuery] int pageSize = 5)
        {
            if (cursor < 0)
            {
                _logger.LogWarning("Invalid cursor id: {cursor} is negative.", [cursor]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Cursor", [$"The cursor cannot contain negative ids ({cursor})."] }
                }));
            }

            if (pageSize < 1)
            {
                _logger.LogWarning("Invalid page size: {pageSize} is negative.", [pageSize]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "PageSize", [$"Page size must be a whole positive number; {pageSize}."] }
                }));
            }

            if (cursor is not null && isNextPage is null)
            {
                _logger.LogWarning("Invalid paging direction {isNextPage}, but cursor was correct.", [isNextPage]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Cursor", [$"Correctly given cursor; {cursor}."] },
                    {"IsNextPage", [$"Paging direction is null, specify forward (true) or backwards (false) direction."]}
                }));
            }

            var reservations = await _reservationService.GetUserReservations(1, cursor, isNextPage, getPast, pageSize);
            return Ok(reservations);
        }
        /// <summary>
        /// Creates a reservation for a specific user, timeslot, and boat
        /// </summary>
        /// <param name="request">The details of the reservation to create</param>
        /// <param name="validator">The validator for the request</param>
        /// <returns>The details of the created reservation</returns>
        [HttpPost]
        [NoQueryParameters]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto request, [FromServices] IValidator<CreateReservationDto> validator)
        {

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails();
                foreach (var error in validationResult.Errors)
                {
                    problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                }
                return ValidationProblem(problemDetails);
            }

            try
            {
                var reservation = await _reservationService.CreateReservation(request.TimeSlotId);
                return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the reservation.", details = ex.Message });
            }
        }


    }
}
