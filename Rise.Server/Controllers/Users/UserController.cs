using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ILogger<UserController> logger, IUserAdminService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserAdminService _userService = userService;

        /// <summary>
        /// Get users by role
        /// </summary>
        /// <param name="role">Role to filter users by (Administrator, Member, Guest).</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>List of users matching the specified role</returns>
        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsersByRole([FromQuery] UserRole role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET api/User?role={Role}&page={Page}&pageSize={PageSize}", role, page, pageSize);

            try
            {
                var users = await _userService.GetUsersByRole(role, page, pageSize);
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
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            _logger.LogInformation("GET api/User/{userId}", userId);
            var details = await _userService.GetUserDetails(userId);
            return Ok(details);
        }

        /// <summary>
        /// Adds member role to a user
        /// </summary>
        /// <param name="request">Dto with id of the user to add member role to</param>
        /// <returns>Result of the operation</returns>
        [HttpPost("role")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMemberRole([FromBody] AddMemberRoleDto request)
        {
            _logger.LogInformation("POST api/User/role for userId: {userId}", request.UserId);
            if (request.Role != UserRole.Member)
            {
                // Only members can be added
                //adding other roles not implemented yet
                return BadRequest("Role must be Member");
            }
            await _userService.AddMemberRole(request.UserId);
            return Ok();
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="userDto">Dto with required user information for registration</param>
        /// <returns>The id of the registered user</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationModelDto userDto)
        {
            _logger.LogInformation("POST api/User/register");
            var userId = await _userService.RegisterUser(userDto);
            return CreatedAtAction(nameof(RegisterUser), userId);
        }

        /// <summary>
        /// Get users by FullName
        /// </summary>
        /// <param name="partialName">A part of a name to use as substring for filtering</param>
        /// <returns>List of users matching that match given partial name</returns>
        [HttpGet("names")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<UserNameDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsersByFullName(
            [FromQuery]
            string? partialName
            )
        {
            _logger.LogInformation("GET api/User/names?partialName={PartialName}", partialName);

            try
            {
                var users = await _userService.GetUsersByFullName(partialName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieve user names with filter: {PartialName}", partialName);
                return BadRequest("Error retrieving user names");
            }
        }
    }
}
