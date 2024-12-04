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

        private BatteryDto? batteryDto;

        public required BatteryDto? BatteryDto
        {
            get => batteryDto;
            set
            {
                batteryDto = value;
                batteryModel = value is null ? DefaultBatteryUpdateDto : new()
                {
                    MentorId = value.MentorId,
                    Type = value.Type,
                };
            }
        }

        public BatteryUpdateDto.Validator batteryValidator = new();

        private Task<BatteryDto> FetchBatteryInfo()
        {
            return BatteryService.GetBattery(Id ?? 1);
        }

        public BatteryUpdateDto batteryModel = DefaultBatteryUpdateDto;

        private static BatteryUpdateDto DefaultBatteryUpdateDto =>
        new()
        {
            MentorId = 0,
            Type = "Lithium",
        };

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        public required MudForm form;

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                try
                {
                    // TODO make localisation
                    // TODO make use of the problemDetails
                    batteryDto = await BatteryService.UpdateBattery(Id ?? 1, batteryModel);
                    Snackbar.Add("Successfully updated battery!", Severity.Success);
                }
                catch (Exception e)
                {
                    Snackbar.Add(e.Message, Severity.Error);
                }

            }

        }
    }
}