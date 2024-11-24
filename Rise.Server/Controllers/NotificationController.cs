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

        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="id">The ID of the notification to mark as read.</param>
        /// <returns></returns>
        /// <response code="204">The notification was successfully marked as read.</response>
        /// <response code="400">The notification ID is invalid.</response>
        /// <response code="404">The notification was not found.</response>
        /// <response code="500">An error occurred while marking the notification as read.</response>
        // [HttpPatch("{id}/read")]
    }
}
