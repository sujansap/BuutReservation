using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Admins.CruisePeriods.TimeSlots
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public required ICruisePeriodService CruisePeriodService { get; set; }

        [Parameter]
        public int? Id { get; set; }
        private TimeSpan? StartTime { get; set; }
        private TimeSpan? EndTime { get; set; }

        private HashSet<CreateTimeSlotDto> TimeSlots { get; set; } = new();
        private CruisePeriodDetailedDto? CruisePeriod { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                CruisePeriod = await CruisePeriodService.GetCruisePeriod(Id.Value);
            }
            await base.OnInitializedAsync();
        }

        public async Task<CruisePeriodDetailedDto> FetchCruisePeriod()
        {
            return await CruisePeriodService.GetCruisePeriod(Id ?? 2);
        }

        private bool CanAddTimeSlot =>
            StartTime.HasValue &&
            EndTime.HasValue &&
            EndTime.Value > StartTime.Value;


        private void AddTimeSlot()
        {
            if (CanAddTimeSlot)
            {

                var timeSlot = new CreateTimeSlotDto
                {
                    Start = TimeOnly.FromTimeSpan(StartTime.Value),
                    End = TimeOnly.FromTimeSpan(EndTime.Value),
                    CruisePeriodId = CruisePeriod?.Id ?? 0
                };

                TimeSlots.Add(timeSlot);

                StartTime = null;
                EndTime = null;

                StateHasChanged();
            }
        }

        private void RemoveTimeSlot(CreateTimeSlotDto timeSlot)
        {
            TimeSlots.Remove(timeSlot);
            StateHasChanged();
        }

        private async Task SaveTimeSlots()
        {
            // TODO: Add your save logic here
        }
    }

}

