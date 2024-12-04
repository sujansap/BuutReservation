using MudBlazor;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Client.Admins.Components
{
    public partial class ReservationsOverview
    {
        private ItemsPageDto<ReservationDto>? Reservations;
        private bool isLoading = true;
        private bool ShowPastReservations = false;
        private int? Cursor;
        private bool? IsNextPage;

        protected override async Task OnInitializedAsync()
        {
            await LoadReservations();
        }

        private async Task LoadReservations()
        {
            try
            {
                isLoading = true;
                Reservations = await ReservationService.GetAllReservations(Cursor, IsNextPage, 10, ShowPastReservations);
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
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error canceling reservation: {ex.Message}", Severity.Error);
            }
        }

        private async Task TogglePastReservations(bool enable)
        {
            if (ShowPastReservations != enable)
            {
                ShowPastReservations = enable;
                Cursor = null;
                IsNextPage = null;
                await LoadReservations();
            }
        }

        private async Task LoadNextPage()
        {
            IsNextPage = true;
            Cursor = Reservations?.NextId;
            await LoadReservations();
        }

        private async Task LoadPreviousPage()
        {
            IsNextPage = false;
            Cursor = Reservations?.PreviousId;
            await LoadReservations();
        }
    }
}