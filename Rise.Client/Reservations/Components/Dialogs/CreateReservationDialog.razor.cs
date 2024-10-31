using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Reservations;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.Dialogs
{
    public partial class CreateReservationDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter]
        public DateOnly Date { get; set; }
        [Parameter]
        public required TimeSlotDto TimeSlot { get; set; }
        [Inject]
        private IReservationService ReservationService { get; set; } = default!;

        public required ReservationCreateDto Reservation { get; set; }
        private DialogState State { get; set; } = DialogState.Overview;
        private UserDto User { get; set; } = new UserDto("John Doe", "john.doe@email.com");

        protected override void OnParametersSet()
        {
            Reservation = new ReservationCreateDto() { TimeSlot = TimeSlot };
        }
        private void Close() => MudDialog.Close();
        private void Cancel() => MudDialog.Cancel();

        private async void CreateReservation()
        {
            State = DialogState.Pay;
            await Task.Delay(3500);
            State = DialogState.Success;
            await ReservationService.CreateReservation(Reservation.TimeSlot.Id);
            StateHasChanged();
        }

        private enum DialogState
        {
            Overview,
            Pay,
            Success
        }


    }
    // TODO move to shared
    public class UserDto(string name, string email)
    {
        public string Name { get; set; } = name;
        public string Email { get; set; } = email;
    }
}
