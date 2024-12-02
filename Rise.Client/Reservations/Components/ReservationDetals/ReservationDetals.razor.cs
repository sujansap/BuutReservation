using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
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

        private bool IsReservationInPast => ReservationDetails?.Date < DateOnly.FromDateTime(DateTime.Now);

        private RenderFragment<string> RenderErrorMessage => (text) => builder =>
        {
            builder.OpenComponent<MudText>(0);
            builder.AddAttribute(1, "Typo", Typo.body1);
            builder.AddAttribute(2, "data-testid", "cancel-reservation-error");
            builder.AddAttribute(3, "ChildContent", (RenderFragment)((b) => b.AddContent(4, text)));
            builder.CloseComponent();
        };

        protected Task<ReservationDetailsDto> GetReservationDetails()
        {
            return ReservationService.GetReservationDetailsAsync(Id);
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
                var message = ex.Message switch
                {
                    "AlreadyCancelled" => Localizer["AlreadyCancelled"],
                    "CancellationTooLate" => Localizer["CancellationTooLate"],
                    _ => Localizer["CancellationError"]
                };
                SnackbarService.Add(RenderErrorMessage(message), Severity.Error);
            }
        }
    }
}


