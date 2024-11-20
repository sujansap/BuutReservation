using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<ReservationController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }



        /// <summary>
        /// Get all the guest users
        /// </summary>
        /// <returns>List of the guest users</returns>
        [HttpGet("guests")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGuestUsers()
        {
            _logger.LogInformation("GET api/user");
            var users = await _userService.GetGuestUsers();
            return Ok(users);
        }
    }
}
