using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Reservations.Components;

public partial class CustomDayComponent
{
    [Parameter]
    public required CalendarItem Context { get; set; }
}
