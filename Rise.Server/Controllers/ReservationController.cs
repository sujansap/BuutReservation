using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

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
        /// <para>To get the first page, set cursor to null. For ther other pages, use the following logic:</para> 
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
        public async Task<IActionResult> GetCurrentUserReservations(
            [FromQuery] int? cursor,
            [FromQuery] bool? isNextPage,
            [FromQuery] bool getPast,
            [FromQuery] int pageSize = 3)
        {
            Console.WriteLine("HERE 22: GetCurrentUserReservations");
            var reservations = await _reservationService.GetUserReservations(1, cursor, isNextPage, getPast, pageSize);
            Console.WriteLine("HERE 23: GetCurrentUserReservations" + reservations.Data.Count());

            return Ok(reservations);
        }
        /// <summary>
        /// Creates a reservation for a specific user, timeslot, and boat
        /// </summary>
        /// <param name="timeSlotId">The ID of the timeslot to reserve</param>
        /// <param name="boatId">The ID of the boat being reserved</param>
        /// <returns>The details of the created reservation</returns>
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto request)
        {
            var userId = 2; //get this from session or token later
            try
            {
                var reservation = await _reservationService.CreateReservation(userId, request.TimeSlotId, request.BoatId);
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
