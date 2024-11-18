using System;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications
{
    public partial class Index
    {
        private NotificationDto SelectedNotification { get; set; }
        private NotificationDto[] Notifications { get; set; } = new NotificationDto[0];

        private void HandleNotificationSelected(NotificationDto notification)
        {
            SelectedNotification = notification;
            notification.IsRead = true;
        }
        protected override void OnInitialized()
        {
            FetchNotifications();
        }
        private void FetchNotifications()
        {
            Notifications = [
                new NotificationDto
                {
                    Severity = Severity.Success.ToString(),
                    Title = "Success",
                    Message = "This is a success message",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsRead = false,
                },
                new NotificationDto
                {
                    Severity = Severity.Info.ToString(),
                    Title = "Info",
                    Message = "This is an info message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)),
                    IsRead = false,
                },
                new NotificationDto
                {
                    Severity = Severity.Warning.ToString(),
                    Title = "Warning",
                    Message = "This is a warning message, but it is really long so it will be truncated",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                    IsRead = false,
                },
                new NotificationDto
                {
                    Severity = Severity.Error.ToString(),
                    Title = "Error",
                    Message = "This is an error message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(5))),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(4))),
                    IsRead = true,
                }
            ];
        }
    }
}
