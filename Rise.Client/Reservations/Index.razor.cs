using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.TimeSlots;


namespace Rise.Client.Reservations
{
    public partial class Index : ComponentBase
    {

        // TODO refactor tab tracer to tabs components
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ChangeTabViaName();
        }
        private static readonly List<string> tabNames = ["calendar", "reservations"];

        private int _tabIndex = 0;
        private int TabIndex
        {
            get => _tabIndex; set
            {
                UpdateSelectedTabInQuery(value);
                _tabIndex = value;
            }
        }

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
                var errorMessage = Localizer["ErrorLoadingReservations"];
                Snackbar.Add(new MarkupString($"<span data-testid='error-message'>{errorMessage}</span>"), Severity.Error);
                return;
            }
        }

        /// <summary>
        /// Which tab is open on the page
        /// </summary>
        private string? CurrentTab { get; set; }

        private void UpdateSelectedTabInQuery(int index)
        {
            string tabName = tabNames[index];
            bool noCurrentTabName = CurrentTab is null;
            if (noCurrentTabName || (CurrentTab is not null && !CurrentTab.Equals(tabName)))
            {
                Dictionary<string, object?> queries = new()
                {
                    ["CurrentTab"] = tabName,
                };
                Navigation.NavigateTo(Navigation.GetUriWithQueryParameters(queries), forceLoad: false, replace: true);
            }

        }

        private void ChangeTabViaName()
        {
            if (CurrentTab is not null)
            {
                CurrentTab = CurrentTab.ToLower();
                int index = tabNames.IndexOf(CurrentTab);
                TabIndex = index < 0 ? 0 : index;
            }
            else
            {
                UpdateSelectedTabInQuery(TabIndex);
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

    }

    public partial class ColoredCalendarItem : CalendarItem
    {
        public Color Color { get; set; } = Color.Primary;
    }
}