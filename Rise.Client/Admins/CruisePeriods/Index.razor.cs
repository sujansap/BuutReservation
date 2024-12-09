using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Admins.CruisePeriods
{
    public partial class Index
    {
        [Inject]
        public required ICruisePeriodService CruisePeriodService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        public required CruisePeriodDetailedDto CruisePeriod { get; set; }

        public async Task<CruisePeriodDetailedDto> FetchCruisePeriod()
        {
            return await CruisePeriodService.GetCruisePeriod(Id ?? 2);
        }
    }
}