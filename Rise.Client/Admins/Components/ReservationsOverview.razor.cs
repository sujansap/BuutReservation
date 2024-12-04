using MudBlazor;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Client.Admins.Components
{
    public partial class ReservationsOverview
    {
        private ItemsPageDto<ReservationDto>? Reservations;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadReservations();
        }

        private async Task LoadReservations()
        {
            try
            {
                Reservations = await ReservationService.GetAllReservations(null, null, 10);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading reservations: {ex.Message}", Severity.Error);
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task CancelReservation(int id)
        {
            try
            {
                await ReservationService.CancelReservationAsync(id);
                Snackbar.Add("Reservation canceled successfully.", Severity.Success);
                await LoadReservations();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error canceling reservation: {ex.Message}", Severity.Error);
            }
        }

        private string SetRowStyle(ReservationDto reservation)
        {
            return reservation.IsDeleted ? "opacity: 0.5;" : "";
        }
    }
}