using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using static Rise.Shared.TimeSlots.CreateTimeSlotDto;
using Validator = Rise.Shared.TimeSlots.CreateTimeSlotDto.Validator;
namespace Rise.Client.Admins.CruisePeriods.TimeSlots
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public required ICruisePeriodService CruisePeriodService { get; set; }


        [Inject]
        private ITimeSlotService TimeSlotService { get; set; } = default!;

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private MudForm? form;

        private Validator validator = new();

        private TimeSpan? StartTime { get; set; }
        private TimeSpan? EndTime { get; set; }
        private CreateTimeSlotDto AllTimeSlotsDto { get; set; } = new CreateTimeSlotDto();
        private CruisePeriodDetailedDto? CruisePeriod { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Id.HasValue)
            {
                CruisePeriod = await CruisePeriodService.GetCruisePeriod(Id.Value);
                AllTimeSlotsDto = new CreateTimeSlotDto
                {
                    CruisePeriodId = Id.Value
                };
            }

        }

        public async Task<CruisePeriodDetailedDto> FetchCruisePeriod()
        {
            return await CruisePeriodService.GetCruisePeriod(Id ?? 2);
        }

        private async Task AddTimeSlot()
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {

                var newTimeSlot = new TimeSlotRange
                {
                    Start = TimeOnly.FromTimeSpan(StartTime.Value),
                    End = TimeOnly.FromTimeSpan(EndTime.Value)
                };

                AllTimeSlotsDto.TimeSlots.Add(newTimeSlot);

                var validationResult = await validator.ValidateAsync(AllTimeSlotsDto);

                if (validationResult.IsValid)
                {

                    StartTime = null;
                    EndTime = null;
                    await form.ResetAsync();
                    StateHasChanged();
                }
                else
                {
                    AllTimeSlotsDto.TimeSlots.Remove(newTimeSlot);
                    foreach (var error in validationResult.Errors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                }
            }
        }

        private void RemoveTimeSlot(TimeSlotRange timeSlot)
        {
            AllTimeSlotsDto.TimeSlots.Remove(timeSlot);
            StateHasChanged();
        }

        private async Task SaveTimeSlots()
        {
            try
            {

                Console.WriteLine("Saving time slots");
                Console.WriteLine(AllTimeSlotsDto.TimeSlots.Count);
                await TimeSlotService.CreateTimeSlot(AllTimeSlotsDto);
                Snackbar.Add("Time slots saved successfully", Severity.Success);


                StateHasChanged();
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to save time slots", Severity.Error);
            }
        }
    }
}