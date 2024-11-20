using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationComponent
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

        [Parameter, EditorRequired]
        public required NotificationDto Notification { get; set; }
        [Parameter]
        public string Class { get; set; } = "";
        [Parameter]
        public EventCallback OnClick { get; set; }

        private async Task HandleClick()
        {
            await OnClick.InvokeAsync();
        }

        private readonly string NotificationIcon = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M12 6C8.69 6 6 8.69 6 12s2.69 6 6 6 6-2.69 6-6-2.69-6-6-6z\"/>";
    }
}