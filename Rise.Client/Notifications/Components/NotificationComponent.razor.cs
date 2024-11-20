using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationComponent
    {
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

        [Parameter, EditorRequired]
        public required NotificationDto Notification { get; set; }

        private async Task HandleClick()
        {
            await OnClick.InvokeAsync();
        }

        // private MudBlazor.Color GetSeverityColor()
        // {
        //     return Severity switch
        //     {
        //         MudBlazor.Severity.Error => MudBlazor.Color.Error,
        //         MudBlazor.Severity.Info => MudBlazor.Color.Info,
        //         MudBlazor.Severity.Success => MudBlazor.Color.Success,
        //         MudBlazor.Severity.Warning => MudBlazor.Color.Warning,
        //         _ => MudBlazor.Color.Default
        //     };
        // }

        // private string GetSeverityIcon()
        // {
        //     return Severity switch
        //     {
        //         MudBlazor.Severity.Error => Icons.Material.Outlined.Error,
        //         MudBlazor.Severity.Info => Icons.Material.Outlined.Info,
        //         MudBlazor.Severity.Success => Icons.Material.Outlined.CheckCircle,
        //         MudBlazor.Severity.Warning => Icons.Material.Outlined.Warning,
        //         _ => Icons.Material.Outlined.Info
        //     };
        // }

        private readonly string NotificationIcon = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M12 6C8.69 6 6 8.69 6 12s2.69 6 6 6 6-2.69 6-6-2.69-6-6-6z\"/>";
    }
}