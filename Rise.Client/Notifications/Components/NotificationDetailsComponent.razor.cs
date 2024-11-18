using Microsoft.AspNetCore.Components;
using Rise.Client.Utils;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationDetailsComponent
    {

        [Parameter]
        public NotificationDto Notification { get; set; }
    }
}