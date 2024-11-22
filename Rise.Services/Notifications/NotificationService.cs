using System;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Notifications;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Shared.Notifications;

namespace Rise.Services.Notifications
{

    public class NotificationService(ApplicationDbContext dbContext) : INotificationService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IEnumerable<NotificationDto>> GetUserNotifications()
        {
            const int userId = 1;
            User user = await _dbContext.Users.FirstAsync(u => u.Id == userId);
            IEnumerable<NotificationDto> notifications = (await _dbContext
                .Users.FirstAsync(u => u.Id == userId))
                .Notifications
                .OrderByDescending(notification => notification.CreatedAt)
                .Select(MapNotificationToDto)
                .ToList();

            return notifications;

        }

        public Task MarkNotificationAsRead(int id)
        {
            throw new NotImplementedException();
        }

        private static NotificationDto MapNotificationToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Severity = (SeverityEnum)notification.Severity,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }

    }

}
