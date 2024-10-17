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
        private List<DateTime> GreyedOutDates = new();

        protected override async Task OnInitializedAsync()
        {
            var dateStart = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
            var dateEnd = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var dateRange = new DateRange(dateStart.ToDateTime(TimeOnly.MinValue), dateEnd.ToDateTime(TimeOnly.MaxValue));
            await UpdateDates(dateRange);
        }

        private async Task OnDateRangeChanged(DateRange dateRange)
        {
            await UpdateDates(dateRange);
        }

        private async Task UpdateDates(DateRange dateRange)
        {
            var response = await TimeSlotService.GetAllTimeSlotsInRange(
                DateOnly.FromDateTime(dateRange.Start.GetValueOrDefault()),
                DateOnly.FromDateTime(dateRange.End.GetValueOrDefault())
            );

            AvailableDays = response.Days
            .Where(day => day.IsSlotAvailable && !day.IsFullyBooked)
            .Select(ConvertToCalendarItems)
            .ToList();

            GreyedOutDates = response.Days.Where(day => day.IsFullyBooked).Select(day => day.Date.ToDateTime(TimeOnly.MinValue)).ToList();
        }

        private ColoredCalendarItem ConvertToCalendarItems(TimeSlotDaySurfaceInfoDto day)
        {
            return new ColoredCalendarItem()
            {
                Start = day.Date.ToDateTime(TimeOnly.MinValue),
                End = day.Date.ToDateTime(TimeOnly.MaxValue),
                Text = day.IsFullyBooked ? "Volzet" : day.IsSlotAvailable ? "" : "Niet beschikbaar",
                Color = day.IsFullyBooked ? Color.Error : day.IsSlotAvailable ? Color.Primary : Color.Warning,
            };
        }
    }

    public partial class ColoredCalendarItem : CalendarItem
    {
        public Color Color { get; set; } = Color.Primary;
    }
}