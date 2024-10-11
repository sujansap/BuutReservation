using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Client.Timeslots
{
    public partial class TimeSlot : ComponentBase
    {
        private IEnumerable<TimeSlotDto>? timeslots;

        [Inject] public required ITimeSlotService TimeSlotService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var specificDate = new DateTime(2024, 10, 12);
            timeslots = await TimeSlotService.GetTimeSlotsByDate(specificDate);
        }
    }
}