using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        /// Gets all reservations for a user
        /// </summary>
        /// <param name="userId">ID of the current user</param>      
        /// <returns>List of reservations</returns>
        [HttpGet("user/{userId}/range")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReservationListDto>))]
        public async Task<IActionResult> GetReservationsForUserInRange(int userId)
        {
            var reservations = await _reservationService.GetCurrentUserReservations(userId);
            return Ok(reservations);
        }
    }
}
