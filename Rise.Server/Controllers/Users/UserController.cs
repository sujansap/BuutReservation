using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Users;
using Rise.Shared.Infrastructure;
using Rise.Shared.Users;

namespace Rise.Server.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(ILogger<UserController> logger, IUserService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Get all the guest users
        /// </summary>
        /// <returns>List of the guest users</returns>
        [HttpGet("guests")]
        [Authorize(Roles = "Administrator")]
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
        /// <param name="request">Dto with id of the user to add member role to</param>
        /// <returns>Result of the operation</returns>
        [HttpPatch("role/member")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMemberRole([FromBody] AddMemberRoleDto request)
        {
            _logger.LogInformation("POST api/User/role for userId: {userId}", request.UserId);
            await _userService.AddMemberRole(request.UserId);
            return Ok();
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="userDto">Dto with required user imformation for registration</param>
        /// <returns>The id of the registered user</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userDto)
        {
            _logger.LogInformation("POST api/User/register");
            var userId = await _userService.RegisterUser(userDto);
            return CreatedAtAction(nameof(RegisterUser), userId);
        }
    }
}
