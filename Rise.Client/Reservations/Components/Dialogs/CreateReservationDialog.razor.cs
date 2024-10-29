using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Reservations;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.Dialogs
{
    public partial class CreateReservationDialog
    {
        [Inject]
        private ISnackbar Snackbar { get; set; }
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public DateOnly Date { get; set; }
        [Parameter]
        public TimeSlotDto? Timeslot { get; set; }

        private bool IsLoading { get; set; } = false;
        public ReservationDto.Create Reservation { get; set; } = new ReservationDto.Create();
        private DialogState State { get; set; } = DialogState.Overview;
        private UserDto User { get; set; } = new UserDto("John Doe", "john.doe@email.com");

        protected override void OnParametersSet()
        {
            Reservation.TimeSlot = Timeslot;
        }
        private void Cancel() => MudDialog.Cancel();

        private async void CreateReservation()
        {
            State = DialogState.Pay;
            await Task.Delay(2000);
            State = DialogState.Success;
            StateHasChanged();
        }

        private enum DialogState
        {
            Overview,
            Pay,
            Success
        }


    }
    public class UserDto(string name, string email)
    {
        public string Name { get; set; } = name;
        public string Email { get; set; } = email;
    }
}
