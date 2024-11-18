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
                },
                new NotificationDto
                {
                    Severity = Severity.Error.ToString(),
                    Title = "Error",
                    Message = "This is an error message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(6))),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(6))),
                    IsRead = false,
                },
                new NotificationDto
                {
                    Severity = Severity.Success.ToString(),
                    Title = "Deployment Complete",
                    Message = "Application successfully deployed to production environment",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Info.ToString(),
                    Title = "System Update",
                    Message = "Scheduled maintenance will occur tomorrow at 2 AM",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2).Add(TimeSpan.FromHours(3))),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2).Add(TimeSpan.FromHours(3))),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Warning.ToString(),
                    Title = "Storage Alert",
                    Message = "Server storage capacity reaching 80%, consider cleanup",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Success.ToString(),
                    Title = "Backup Complete",
                    Message = "Weekly backup completed successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Info.ToString(),
                    Title = "New Feature",
                    Message = "Dark mode is now available in your settings",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Error.ToString(),
                    Title = "Connection Failed",
                    Message = "Unable to connect to secondary database server",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5).Add(TimeSpan.FromHours(6))),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5).Add(TimeSpan.FromHours(6))),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Warning.ToString(),
                    Title = "CPU Usage High",
                    Message = "System CPU usage exceeded 90% for 5 minutes",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Success.ToString(),
                    Title = "Test Suite Passed",
                    Message = "All integration tests completed successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Info.ToString(),
                    Title = "Profile Updated",
                    Message = "Your profile information has been updated successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Warning.ToString(),
                    Title = "SSL Certificate",
                    Message = "SSL Certificate will expire in 30 days",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Error.ToString(),
                    Title = "Payment Failed",
                    Message = "Monthly subscription payment processing failed",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Success.ToString(),
                    Title = "Report Generated",
                    Message = "Monthly analytics report has been generated",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(11)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(11)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Info.ToString(),
                    Title = "Team Meeting",
                    Message = "Reminder: Team meeting scheduled for tomorrow at 10 AM",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Warning.ToString(),
                    Title = "API Rate Limit",
                    Message = "API rate limit reached 85% of maximum allocation",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(13)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(13)),
                    IsRead = true,
                },
                new NotificationDto
                {
                    Severity = Severity.Error.ToString(),
                    Title = "Security Alert",
                    Message = "Multiple failed login attempts detected from unknown IP",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(14)),
                    UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(14)),
                    IsRead = true,
                }
            ];
        }
    }
}
