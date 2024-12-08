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
                Snackbar.Add(Localizer["ErrorLoadingReservations", ex.Message], Severity.Error);
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
                Snackbar.Add(Localizer["ReservationCancelledSuccess"], Severity.Success);
                await LoadReservations();
            }
            catch (Exception ex)
            {
                Snackbar.Add(Localizer["ErrorCancelingReservation", ex.Message], Severity.Error);
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
            if (Reservations?.NextId == null)
            {
                Snackbar.Add(Localizer["NoMorePages"], Severity.Warning);
                return;
            }

            IsNextPage = true;
            Cursor = Reservations?.NextId;
            await LoadReservations();
        }

        private async Task LoadPreviousPage()
        {
            if (Reservations?.PreviousId == null || Reservations?.IsFirstPage == true)
            {
                Snackbar.Add(Localizer["AlreadyOnFirstPage"], Severity.Warning);
                return;
            }

            IsNextPage = false;
            Cursor = Reservations?.PreviousId;
            await LoadReservations();
        }
    }
}