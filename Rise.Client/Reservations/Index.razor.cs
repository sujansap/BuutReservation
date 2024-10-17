using Microsoft.AspNetCore.Components;
using Heron.MudCalendar;
using MudBlazor;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations
{
    public partial class Index : ComponentBase
    {
        /// <summary>
        /// The start date if the date range, by default the current's month start date
        /// </summary>
        private readonly DateOnly defaultStartDay = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        /// <summary>
        /// The end date if the date range, by default the current's month end date
        /// </summary>
        private readonly DateOnly defaultEndDay = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        [SupplyParameterFromQuery]
        /// <summary>
        /// The actual start date for the date range
        /// </summary>
        private DateOnly? StartDate { get; set; }

        [SupplyParameterFromQuery]
        /// <summary>
        /// The actual start date for the date range
        /// </summary>
        private DateOnly? EndDate { get; set; }

        [Inject]
        private ITimeSlotService TimeSlotService { get; set; } = default!;

        /// <summary>
        /// All available days on the calendar
        /// </summary>
        private List<ColoredCalendarItem> AvailableDays = [];
        /// <summary>
        /// All unavailable days on the calendar
        /// </summary>
        private List<DateTime> GreyedOutDates = [];

        protected override async Task OnInitializedAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                NavigateToDefaultDateRange();
            }
            else
            {
                // TODO fix that current day gets loaded in
                await UpdateDates();
            }
        }

        /// <summary>
        /// Handle when calendar date range changes
        /// </summary>
        /// <param name="dateRange">The new date range</param>
        private async Task OnDateRangeChanged(DateRange dateRange)
        {
            if (dateRange.Start.HasValue && dateRange.End.HasValue && (DateOnly.FromDateTime(dateRange.Start.Value.Date) != StartDate || DateOnly.FromDateTime(dateRange.End.Value.Date) != EndDate))
            {
                StartDate = DateOnly.FromDateTime(dateRange.Start.Value);
                EndDate = DateOnly.FromDateTime(dateRange.End.Value);
                NavigateToDateRange();
                await UpdateDates();
            }
        }

        /// <summary>
        /// Navigate to the default date range
        /// </summary>
        private void NavigateToDefaultDateRange()
        {
            StartDate = defaultStartDay;
            EndDate = defaultEndDay;
            NavigateToDateRange();
        }

        /// <summary>
        /// Navigate to the current date range
        /// </summary>
        private void NavigateToDateRange()
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            Navigation.NavigateTo(Navigation.GetUriWithQueryParameters(
                            new Dictionary<string, object?>
                            {
                                ["StartDate"] = StartDate.Value.ToString("yyyy-MM-dd"),
                                ["EndDate"] = EndDate.Value.ToString("yyyy-MM-dd")
                            }), forceLoad: false);
#pragma warning restore CS8629 // Nullable value type may be null.
        }

        /// <summary>
        /// Update known dates via the api
        /// </summary>
        /// <returns></returns>
        private async Task UpdateDates()
        {

            if (!StartDate.HasValue || !EndDate.HasValue)
                return;

            try
            {
                TimeSlotRangeInfoDto response = await TimeSlotService.GetAllTimeSlotsInRange(
                StartDate.Value,
                EndDate.Value
            );


                AvailableDays = response.Days
                .Where(day => day.IsSlotAvailable && !day.IsFullyBooked)
                .Select(ConvertToCalendarItems)
                .ToList();

                GreyedOutDates = response.Days.Where(day => day.IsFullyBooked).Select(day => day.Date.ToDateTime(TimeOnly.MinValue)).ToList();
            }

            catch
            {
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