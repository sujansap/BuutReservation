using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Rise.Client.Common;
using Rise.Client.Localization.Reservations;
using Rise.Shared;
using Rise.Shared.Reservations;
using Serilog;

namespace Rise.Client.Reservations.Components.ReservationDetals
{
    public partial class ReservationDetals : ComponentBase
    {

        public required AsyncData<ReservationDetailsDto> AsyncDataRef { get; set; }
        protected ReservationDetailsDto? ReservationDetails { get; set; }



        [Parameter]
        public int Id { get; set; }

        [Inject]
        public required ISnackbar SnackbarService { get; set; }


        [Inject]


        public required IReservationService ReservationService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        protected Task<ReservationDetailsDto> GetReservationDetails()
        {
            return ReservationService.GetReservationDetailsAsync(Id);
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/reservations?CurrentTab=reservations");
        }



        private async Task CancelReservation()
        {
            if (ReservationDetails is null)
                return;
            try
            {
                await ReservationService.CancelReservationAsync(ReservationDetails.Id);
                ReservationDetails.IsDeleted = true;
                StateHasChanged();


                NavigationManager.NavigateTo("/reservations?CurrentTab=reservations");
            }
            catch (Exception ex)
            {
                Log.Error($"Error cancelling reservation: {ex.Message}");
                SnackbarService.Add(RenderErrorMessage(ex.Message), Severity.Error);
            }
        }
    }


}


