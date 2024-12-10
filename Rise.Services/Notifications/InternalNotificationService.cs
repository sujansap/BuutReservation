using Ardalis.GuardClauses;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Shared.Notifications;

namespace Rise.Services.Notifications
{

    public class InternalNotificationService(ApplicationDbContext dbContext, ILogger<InternalNotificationService> logger)
        : IInternalNotificationService
    {
        public Task SendNotificationToUser(int userId, string title, string message, SeverityEnum severity)
        {
            try
            {
                logger.LogInformation($"Sending notification to user {userId} with severity {severity} and title {title}");
                Notification notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Severity = (int)severity,
                    User = dbContext.Users.Find(userId) ?? throw new NotFoundException(userId.ToString(), typeof(User).ToString())
                };

                dbContext.Notifications.Add(notification);

                return dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending notification", ex);
            }
        }


    }

}
