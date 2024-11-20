using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications
{
    public partial class Index
    {
        [Inject]
        public required INotificationService NotificationService { get; set; }
        private NotificationDto? SelectedNotification { get; set; }
        private IEnumerable<NotificationDto> Notifications { get; set; } = [];

        public required AsyncData<IEnumerable<NotificationDto>> AsyncDataRef { get; set; }

        private void HandleNotificationSelected(NotificationDto notification)
        {
            SelectedNotification = notification;
            notification.IsRead = true;
        }
        private Task<IEnumerable<NotificationDto>> FetchNotifications()
        {
            return NotificationService.GetUserNotifications();
        }

    }
}
