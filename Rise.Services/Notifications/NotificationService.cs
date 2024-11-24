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

        public async Task<IEnumerable<NotificationDto>> GetUserNotifications(int? limit)
        {
            const int userId = 1;
            IEnumerable<NotificationDto> notifications = (await _dbContext.Users.Include(user => user.Notifications)
                .FirstAsync(user => user.Id == userId))
                .Notifications.OrderByDescending(notification => notification.CreatedAt)
                .Select(MapNotificationToDto);

            if (limit.HasValue)
            {
                notifications = notifications.Take(limit.Value);
            }

            return notifications.ToList();
        }

        public Task MarkNotificationAsRead(int id)
        {
            const int userId = 1;
            int count = _dbContext.Users.Where(user => user.Id == userId)
                .SelectMany(user => user.Notifications)
                .Where(notification => notification.Id == id)
                .Count();

            Console.WriteLine($"Notification count: {count}");
            return _dbContext.Users.Where(user => user.Id == userId)
                .SelectMany(user => user.Notifications)
                .Where(notification => notification.Id == id)
                .ForEachAsync(notification => notification.IsRead = true);
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
