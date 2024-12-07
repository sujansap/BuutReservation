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
        public required ISnackbar Snackbar { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private MudForm form;

        // private Validator validator = new();

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

        private async Task AddTimeSlot()
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                var timeSlot = new CreateTimeSlotDto
                {
                    CruisePeriodId = CruisePeriod?.Id ?? 0,
                    TimeSlots = new List<TimeSlotRange>
                    {
                        new TimeSlotRange
                        {
                            Start = TimeOnly.FromTimeSpan(StartTime.Value),
                            End = TimeOnly.FromTimeSpan(EndTime.Value)
                        }
                    }
                };

                var existingSlots = TimeSlots.SelectMany(ts => ts.TimeSlots).ToList();

                var validator = new CreateTimeSlotDto.Validator(existingSlots);
                var validationResult = await validator.ValidateAsync(timeSlot);

                if (validationResult.IsValid)
                {
                    TimeSlots.Add(timeSlot);
                    StartTime = null;
                    EndTime = null;
                    await form.ResetAsync();
                    StateHasChanged();
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                }
            }
        }

        private void RemoveTimeSlot(CreateTimeSlotDto timeSlot)
        {
            TimeSlots.Remove(timeSlot);
            StateHasChanged();
        }

        private async Task SaveTimeSlots()
        {
            try
            {
                foreach (var timeSlot in TimeSlots)
                {
                    //await CruisePeriodService.CreateTimeSlot(timeSlot);
                }

                Snackbar.Add("Time slots saved successfully", Severity.Success);

                TimeSlots.Clear();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Snackbar.Add("Failed to save time slots", Severity.Error);
            }
        }
    }
}