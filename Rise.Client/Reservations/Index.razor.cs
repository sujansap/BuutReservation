using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private ITimeSlotService TimeSlotService { get; set; } = default!;

        /// <summary>
        /// All unavailable days on the calendar
        /// </summary>
        private Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> AvailableDays { get; set; } = [];
        private List<ColoredCalendarItem> ReservationsOfCurrentUser = new List<ColoredCalendarItem>();

        private DateOnly? SelectedDate { get; set; }

        /// <summary>
        /// Handle when calendar date range changes
        /// </summary>
        /// <param name="dateRange">The new date range</param>
        private async Task OnDateRangeChanged(DateRange dateRange)
        {
            if (dateRange.Start.HasValue && dateRange.End.HasValue)
            {
                DateOnly startDate = DateOnly.FromDateTime(dateRange.Start.Value);
                DateOnly endDate = DateOnly.FromDateTime(dateRange.End.Value);
                await UpdateDates(startDate, endDate);
            }
        }

        /// <summary>
        /// Update known dates via the api
        /// </summary>
        /// <returns></returns>
        private async Task UpdateDates(DateOnly startDate, DateOnly endDate)
        {

            try
            {
                TimeSlotRangeInfoDto response = await TimeSlotService.GetAllTimeSlotsInRange(
                startDate,
                endDate
            );


                AvailableDays = response.Days
                .Where(day => day.IsSlotAvailable)
                .ToDictionary(day => day.Date, day => day);

                ReservationsOfCurrentUser = response.Days.Where(day => day.IsBookedByUser).Select(ConvertToCalendarItems).ToList();
            }

            catch
            {
                // TODO this error message is not localized
                var errorMessage = "Er is iets mis gegaan bij het ophalen van de beschikbare dagen";
                Snackbar.Add(new MarkupString($"<span data-testid='error-message'>{errorMessage}</span>"), Severity.Error);
                return;
            }
        }

        /// <summary>
        /// Converts calendar item to highlight on calendar using day info
        /// </summary>
        /// <param name="day">day info about the events</param>
        /// <returns>calendar item</returns>
        private static ColoredCalendarItem ConvertToCalendarItems(TimeSlotDaySurfaceInfoDto day)
        {
            return new ColoredCalendarItem()
            {
                Start = day.Date.ToDateTime(TimeOnly.MinValue),
                End = day.Date.ToDateTime(TimeOnly.MaxValue),
                Text = "",
                Color = Color.Primary,
            };
        }

        /// <summary>
        /// When a day is being selected
        /// </summary>
        /// <param name="date">The clicked date</param>
        /// <returns></returns>
        private void OnCellClicked(DateTime date)
        {
            if (AvailableDays.ContainsKey(DateOnly.FromDateTime(date)))
            {
                SelectedDate = DateOnly.FromDateTime(date);
            }
            else
            {
                SelectedDate = null;
            }
        }

    }

    public partial class ColoredCalendarItem : CalendarItem
    {
        public Color Color { get; set; } = Color.Primary;
    }
}