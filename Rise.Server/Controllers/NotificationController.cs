using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Notifications;

namespace Rise.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets all notifications for the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotificationDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentUserNotifications([FromQuery] int? limit)
        {
            if (limit.HasValue && limit.Value < 0)
            {
                _logger.LogWarning("Invalid limit: {limit} is negative.", [limit.Value]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Limit", [$"The limit cannot contain negative values ({limit})."] }
                }));
            }
            try
            {
                var notifications = await _notificationService.GetUserNotifications(limit);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching notifications for the current user.");
                return Problem("An error occurred while fetching notifications for the current user.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
