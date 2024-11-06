using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.TimeSlotList
{
    public partial class TimeSlotsList
    {
        public required AsyncData<IEnumerable<TimeSlotDto>> AsyncDataRef { get; set; }
        private IEnumerable<TimeSlotDto> TimeSlots { get; set; } = [];

        [Inject]
        public required ITimeSlotService TimeSlotService { get; set; }

        [Parameter]
        public required DateOnly SelectedDate { get; set; }

        private Task<IEnumerable<TimeSlotDto>> FetchTimeSlots()
        {
            return TimeSlotService.GetTimeSlotsByDate(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day);
        }
    }
}
