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
            IQueryable<Notification> query = _dbContext.Users.Where(user => user.Id == userId)
                .SelectMany(user => user.Notifications)
                .OrderByDescending(notification => notification.CreatedAt);

            var dtoQuery = query.Select(notification => MapNotificationToDto(notification));

            if (limit.HasValue)
            {
                dtoQuery = dtoQuery.Take(limit.Value);
            }

            List<NotificationDto> notifications = await dtoQuery.ToListAsync();

            return notifications;
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
