using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Notifications.Components
{
    public partial class NotificationComponent
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

        [Parameter]
        public string Title { get; set; } = "";

        [Parameter]
        public string Message { get; set; } = "";

        [Parameter, EditorRequired]
        public required MudBlazor.Severity Severity { get; set; }

        [Parameter, EditorRequired]
        public required DateTime TimeStamp { get; set; }
        [Parameter, EditorRequired]
        public required bool IsRead { get; set; }


        private MudBlazor.Color GetSeverityColor()
        {
            return Severity switch
            {
                MudBlazor.Severity.Error => MudBlazor.Color.Error,
                MudBlazor.Severity.Info => MudBlazor.Color.Info,
                MudBlazor.Severity.Success => MudBlazor.Color.Success,
                MudBlazor.Severity.Warning => MudBlazor.Color.Warning,
                _ => MudBlazor.Color.Default
            };
        }

        private string GetSeverityIcon()
        {
            return Severity switch
            {
                MudBlazor.Severity.Error => Icons.Material.Outlined.Error,
                MudBlazor.Severity.Info => Icons.Material.Outlined.Info,
                MudBlazor.Severity.Success => Icons.Material.Outlined.CheckCircle,
                MudBlazor.Severity.Warning => Icons.Material.Outlined.Warning,
                _ => Icons.Material.Outlined.Info
            };
        }

        private string FormatTimeStamp(DateTime timeStamp)
        {
            //If the timestamp is today, return the time
            if (timeStamp.Date == DateTime.Today)
            {
                return timeStamp.ToString("t");
            }

            //If the timestamp was yesterday, return "Yesterday" + time
            if (timeStamp.Date == DateTime.Today.AddDays(-1))
            {
                return "Gisteren om " + timeStamp.ToString("t");
            }

            //If the timestamp is within the last 7 days, return the day of the week and the time
            if (timeStamp.Date >= DateTime.Today.AddDays(-7))
            {
                //return the day of the week and the time
                return timeStamp.ToString("dddd HH:mm");
            }

            //Otherwise, return the date
            return timeStamp.ToString("g");
        }

        private readonly string CircleIcon = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M12 6C8.69 6 6 8.69 6 12s2.69 6 6 6 6-2.69 6-6-2.69-6-6-6z\"/>";
    }
}