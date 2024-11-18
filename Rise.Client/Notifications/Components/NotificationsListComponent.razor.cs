using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationsListComponent
    {
        [Parameter, EditorRequired]
        public required NotificationDto[] Notifications { get; set; }

        [Parameter]
        public EventCallback<NotificationDto> OnNotificationSelected { get; set; }

        private Severity GetSeverityFromString(string severity)
        {
            return severity switch
            {
                "Error" => Severity.Error,
                "Info" => Severity.Info,
                "Success" => Severity.Success,
                "Warning" => Severity.Warning,
                _ => Severity.Info
            };
        }
    }
}