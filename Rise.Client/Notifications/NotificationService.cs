using System;
using System.Net.Http.Json;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications
{
    public class NotificationService(HttpClient httpClient) : INotificationService
    {
        private readonly HttpClient _httpClient = httpClient;
        public async Task<IEnumerable<NotificationDto>> GetUserNotifications()
        {
            IEnumerable<NotificationDto> result = await _httpClient.GetFromJsonAsync<IEnumerable<NotificationDto>>("me")
                ?? throw new Exception("Failed to get notifications");
            return result;
        }

        public Task MarkNotificationAsRead(int notificationId)
        {
            // unimplemented 
            return Task.CompletedTask;
        }
    }

}

