using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationsListComponent
    {
        [Parameter, EditorRequired]
        public required IEnumerable<NotificationDto> Notifications { get; set; }

        [Parameter]
        public EventCallback<NotificationDto> OnNotificationSelected { get; set; }
        [Parameter]
        public NotificationDto? SelectedNotification { get; set; }
    }
}