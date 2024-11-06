using System.Text.Json;
using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components
{
    public partial class TimeSlotOverview
    {
        public required AsyncData<TimeSlotRangeInfoDto> AsyncDateRangeRef { get; set; }

        public TimeSlotRangeInfoDto? TimeSlotRangeInfo { get; set; }

        [Inject]
        private ITimeSlotService TimeSlotService { get; set; } = default!;

        /// <summary>
        /// All unavailable days on the calendar
        /// </summary>
        private Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> AvailableDays { get; set; } = [];
        private List<ColoredCalendarItem> ReservationsOfCurrentUser = [];

        private DateOnly? SelectedDate { get; set; }

        private DateRange dateRange = new();

        /// <summary>
        /// Handle when calendar date range changes
        /// </summary>
        /// <param name="dateRange">The new date range</param>
        private async Task OnDateRangeChanged(DateRange dateRange)
        {
            if (dateRange.Start.HasValue && dateRange.End.HasValue && !dateRange.Equals(this.dateRange))
            {
                this.dateRange = dateRange;


                await AsyncDateRangeRef.FetchData();

                DisplayDateRange();
            }
        }

        /// <summary>
        /// Fetch dates in given range from api
        /// </summary>
        private Task<TimeSlotRangeInfoDto> FetchDateRange()
        {
            if (dateRange.Start.HasValue && dateRange.End.HasValue)
            {
                DateOnly startDate = DateOnly.FromDateTime(dateRange.Start.Value);
                DateOnly endDate = DateOnly.FromDateTime(dateRange.End.Value);
                return TimeSlotService.GetAllTimeSlotsInRange(
                    startDate,
                    endDate);
            }

            return Task.FromResult(TimeSlotRangeInfo ?? new TimeSlotRangeInfoDto(0, []));
        }

        private void DisplayDateRange()
        {

            if (TimeSlotRangeInfo is not null)
            {
                AvailableDays = TimeSlotRangeInfo.Days
                .Where(day => day.IsSlotAvailable)
                .ToDictionary(day => day.Date, day => day);

                ReservationsOfCurrentUser = TimeSlotRangeInfo.Days.Where(day => day.IsBookedByUser)
                .Select(ConvertToCalendarItems)
                .ToList();

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

        // TODO refactor tab tracer to tabs components
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

        [SupplyParameterFromQuery]
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

    }

    public partial class ColoredCalendarItem : CalendarItem
    {
        public Color Color { get; set; } = Color.Primary;
    }
}