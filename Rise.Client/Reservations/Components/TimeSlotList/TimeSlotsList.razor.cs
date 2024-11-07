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
        [Parameter, EditorRequired]
        public required Func<Task> FetchAvailableDays { get; set; }

        private DateOnly? _previousDate = default;

        [Parameter]
        public required DateOnly SelectedDate { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (_previousDate is null)
            {
                _previousDate = SelectedDate;
            }
            else if (!_previousDate.Equals(SelectedDate))
            {
                _previousDate = SelectedDate;
                await AsyncDataRef.FetchData();
            }
        }

        private Task<IEnumerable<TimeSlotDto>> FetchTimeSlots()
        {
            return TimeSlotService.GetTimeSlotsByDate(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day);
        }

        private async Task RefetchTimeSlotOverviewData()
        {
            await AsyncDataRef.FetchData();
            await FetchAvailableDays();
        }
    }
}
