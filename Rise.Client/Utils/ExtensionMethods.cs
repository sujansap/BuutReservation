using MudBlazor;

namespace Rise.Client.Utils
{
    public static class ExtensionMethods
    {
        private const string universalDateFormat = "yyyy-MM-dd";
        public static string ToUniversalStringDate(this DateOnly date)
        {
            return date.ToString(universalDateFormat);
        }

        public static string ToRelativeDateString(this DateTime date)
        {
            //If the timestamp is today, return the time
            if (date.Date == DateTime.Today)
            {
                return date.ToString("t");
            }

            //If the timestamp was yesterday, return "Yesterday" + time
            if (date.Date == DateTime.Today.AddDays(-1))
            {
                return "Gisteren om " + date.ToString("t");
            }

            //If the timestamp is within the last 7 days, return the day of the week and the time
            if (date.Date >= DateTime.Today.AddDays(-7))
            {
                //return the day of the week and the time
                return date.ToString("dddd HH:mm");
            }

            //Otherwise, return the date
            return date.ToString("g");
        }

        public static MudBlazor.Color Color(this MudBlazor.Severity severity)
        {
            return severity switch
            {
                MudBlazor.Severity.Error => MudBlazor.Color.Error,
                MudBlazor.Severity.Info => MudBlazor.Color.Info,
                MudBlazor.Severity.Success => MudBlazor.Color.Success,
                MudBlazor.Severity.Warning => MudBlazor.Color.Warning,
                _ => MudBlazor.Color.Default
            };
        }

        public static string Icon(this MudBlazor.Severity severity)
        {
            return severity switch
            {
                MudBlazor.Severity.Error => Icons.Material.Outlined.Error,
                MudBlazor.Severity.Info => Icons.Material.Outlined.Info,
                MudBlazor.Severity.Success => Icons.Material.Outlined.CheckCircle,
                MudBlazor.Severity.Warning => Icons.Material.Outlined.Warning,
                _ => Icons.Material.Outlined.Info
            };
        }

        public static MudBlazor.Severity ToSeverity(this string severity)
        {
            return severity.ToLower() switch
            {
                "info" => MudBlazor.Severity.Info,
                "success" => MudBlazor.Severity.Success,
                "warning" => MudBlazor.Severity.Warning,
                "error" => MudBlazor.Severity.Error,
                _ => MudBlazor.Severity.Normal
            };
        }
    }
}

