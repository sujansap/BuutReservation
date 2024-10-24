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
        public ReservationDto.Create Reservation { get; set; } = new ReservationDto.Create();

        protected override void OnParametersSet()
        {
            Reservation.TimeSlot = Timeslot;
        }
        private void Cancel() => MudDialog.Cancel();

        private void CreateReservation()
        {
            Snackbar.Add("Reservatie aangemaakt", Severity.Success);
            MudDialog.Close();
        }
    }
}
