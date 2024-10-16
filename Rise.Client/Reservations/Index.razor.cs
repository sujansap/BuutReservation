using Microsoft.AspNetCore.Components;
using Heron.MudCalendar;
using MudBlazor;
using Rise.Client.TimeSlots;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private ITimeSlotService TimeSlotService { get; set; }

        private List<ColoredCalendarItem> AvailableDays = new();

        protected override async Task OnInitializedAsync()
        {
            var response = await TimeSlotService.GetAllTimeSlotsInRange(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1), new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
            AvailableDays = response.Days.Select(ConvertToCalendarItems).ToList();
        }

        private async Task OnDateRangeChanged(DateRange dateRange)
        {
            var response = await TimeSlotService.GetAllTimeSlotsInRange(
                DateOnly.FromDateTime(dateRange.Start.GetValueOrDefault()),
                DateOnly.FromDateTime(dateRange.End.GetValueOrDefault())
            );

            AvailableDays = response.Days
            .Where(day => day != null && !day.IsFullyBooked)
            .Select(ConvertToCalendarItems)
            .Where(item => item != null)
            .ToList();
        }
        private string GetColor(Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";

        private ColoredCalendarItem ConvertToCalendarItems(TimeSlotDaySurfaceInfoDto day)
        {
            return new ColoredCalendarItem()
            {
                Start = day.Date.ToDateTime(TimeOnly.MinValue),
                End = day.Date.ToDateTime(TimeOnly.MaxValue),
                Text = day.IsSlotAvailable ? "Beschikbaar" : "Volzet",
                Color = day.IsSlotAvailable ? Color.Primary : Color.Warning
            };
        }

        private class ColoredCalendarItem : CalendarItem
        {
            public Color Color { get; set; } = Color.Primary;
        }
    }
}