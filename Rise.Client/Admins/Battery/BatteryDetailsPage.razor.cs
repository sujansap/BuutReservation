using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Boats;

namespace Rise.Client.Admins.Battery
{
    public partial class BatteryDetailsPage
    {
        [Inject]
        public required IBatteryService BatteryService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        public BatteryUpdateDto.Validator batteryValidator = new();

        private static BatteryUpdateDto DefaultBatteryUpdateDto =>
        new()
        {
            MentorId = 0,
            Type = "Lithium",
        };


        public static BatteryUpdateDto BatteryToUpdateBattery(BatteryDto batteryDto)
        {
            return batteryDto is null ? DefaultBatteryUpdateDto : new()
            {
                MentorId = batteryDto.MentorId,
                Type = batteryDto.Type,
            };
        }

        private Task<BatteryDto> FetchBatteryInfo()
        {
            return BatteryService.GetBattery(Id ?? 1);
        }

        public BatteryUpdateDto batteryModel = DefaultBatteryUpdateDto;

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        private async Task<BatteryDto> HandleSubmit(BatteryUpdateDto batteryDetails)
        {
            return await BatteryService.UpdateBattery(Id ?? 1, batteryDetails);
        }
    }
}