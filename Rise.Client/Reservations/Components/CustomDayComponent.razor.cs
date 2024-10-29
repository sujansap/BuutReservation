using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Reservations.Components
{
    public partial class CustomDayComponent
    {
        [Parameter]
        public required CalendarItem Context { get; set; }
    }

    public partial class ColoredCalendarItem : CalendarItem
    {
        public Color Color { get; set; } = Color.Primary;
    }
}


