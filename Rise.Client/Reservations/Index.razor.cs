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
        private List<DateTime> GreyedOutDates = [];

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

                GreyedOutDates = response.Days.Where(day => day.IsFullyBooked || !day.IsSlotAvailable).Select(day => day.Date.ToDateTime(TimeOnly.MinValue)).ToList();
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
        /// When a day is being selected
        /// </summary>
        /// <param name="date">The clicked date</param>
        /// <returns></returns>
        private void OnCellClicked(DateTime date)
        {
            SelectedDate = DateOnly.FromDateTime(date);
        }
    }
}