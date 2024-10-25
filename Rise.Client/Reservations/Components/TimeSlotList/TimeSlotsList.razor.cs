using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.TimeSlotList
{
    public partial class TimeSlotsList
    {
        private IEnumerable<TimeSlotDto> timeSlots = [];

        [Inject]
        public required ITimeSlotService TimeSlotService { get; set; }

        [Parameter]
        public DateOnly SelectedDate { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await UpdateTimeSlots();
        }

        private async Task UpdateTimeSlots()
        {
            timeSlots = await TimeSlotService.GetTimeSlotsByDate(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day);
        }
    }
}
