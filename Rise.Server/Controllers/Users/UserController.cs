using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Users;
using Rise.Shared.Infrastructure;
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
            _logger.LogInformation("GET api/User/guests");
            var users = await _userService.GetGuestUsers();
            return Ok(users);
        }

        /// <summary>
        /// Gets the details of a user
        /// </summary>
        /// <returns>The details of a user</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            _logger.LogInformation("GET api/User/{userId}", userId);
            var details = await _userService.GetUserDetails(userId);
            return Ok(details);
        }

        /// <summary>
        /// Adds memeber role to a user
        /// </summary>
        /// <param name="userId">ID of the user whome you want to make a memeber</param>
        /// <returns>Result of the operation</returns>
        [HttpPost("role/member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUserRole([FromBody] int userId)
        {
            _logger.LogInformation("POST api/User/role for userId: {userId}", userId);
            // TODO: add role to the user with the given userId using auth0 management api
            return Ok();
        }


    }
}
