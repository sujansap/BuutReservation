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
        /// Get users by role
        /// </summary>
        /// <param name="role">Role to filter users by (Administrator, Member, Guest). If not specified, returns all users.</param>
        /// <returns>List of users matching the specified role</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsersByRole([FromQuery] string? role = null)
        {
            _logger.LogInformation("GET api/User/users?role={Role}", role);

            try
            {
                IEnumerable<UserDto> users = new List<UserDto>();

                if (string.IsNullOrEmpty(role))
                {
                    return BadRequest("Can't get users without specifying a role");
                }
                else
                {
                    users = await _userService.GetUsersByRole(role);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for role: {Role}", role);
                return BadRequest("Error retrieving users");
            }
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
        /// <returns>Result of the operation</returns>
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
